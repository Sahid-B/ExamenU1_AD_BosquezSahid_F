(function () {
    "use strict";

    const container = document.getElementById("content-table");
    const status = document.getElementById("realtime-status");

    if (!container || !window.signalR) {
        return;
    }

    function setStatus(text, cssClass) {
        if (!status) {
            return;
        }
        status.textContent = text;
        status.className = "badge fs-6 " + cssClass;
    }

    function currentPage() {
        const holder = container.querySelector("[data-page]");
        return holder ? holder.getAttribute("data-page") : "1";
    }

    let refreshTimer = null;
    let refreshing = false;
    let refreshAgain = false;

    async function refreshTable() {
        if (refreshing) {
            refreshAgain = true;
            return;
        }

        refreshing = true;
        try {
            const response = await fetch("/Content/Table?page=" + encodeURIComponent(currentPage()), {
                headers: { "X-Requested-With": "XMLHttpRequest" },
                cache: "no-store"
            });

            if (response.ok) {
                container.innerHTML = await response.text();
            }
        } catch (error) {
            console.error("No se pudo actualizar el contenido.", error);
        } finally {
            refreshing = false;
            if (refreshAgain) {
                refreshAgain = false;
                refreshTable();
            }
        }
    }

    function scheduleRefresh() {
        clearTimeout(refreshTimer);
        refreshTimer = setTimeout(refreshTable, 150);
    }

    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/contentHub")
        .withAutomaticReconnect()
        .build();

    connection.on("ContentChanged", function () {
        scheduleRefresh();
    });

    connection.onreconnecting(function () {
        setStatus("Reconectando...", "bg-warning text-dark");
    });

    connection.onreconnected(function () {
        setStatus("En vivo", "bg-success");
        scheduleRefresh();
    });

    connection.onclose(function () {
        setStatus("Desconectado", "bg-danger");
    });

    async function start() {
        try {
            await connection.start();
            setStatus("En vivo", "bg-success");
        } catch (error) {
            setStatus("Reconectando...", "bg-warning text-dark");
            setTimeout(start, 2000);
        }
    }

    start();
})();
