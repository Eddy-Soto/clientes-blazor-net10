window.clientesModal = {
    abrir: element => bootstrap.Modal.getOrCreateInstance(element, { backdrop: "static", keyboard: false }).show(),
    cerrar: element => bootstrap.Modal.getInstance(element)?.hide()
};
