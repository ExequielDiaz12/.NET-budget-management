﻿function InicializarFormularioTransacciones(ObtenerCategorias) {
    $("#TipoOperacionId").change(async function () {
        const valorSeleccionado = $(this).val();
        const respuesta = await fetch(UrlObtenerCategorias, {
            method: 'POST',
            body: valorSeleccionado,
            headers: {
                'Content-type': 'application/json'
            }
        });

        const json = await respuesta.json();
        const opciones = json.map(categoria => `<option value=${categoria.value}>${categoria.text}</option>`);
        $("#CategoriaId").html(opciones);
    })
}