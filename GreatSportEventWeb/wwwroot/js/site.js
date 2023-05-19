﻿// Функция для обновления таблицы с очисткой кэша.
function refreshCache(modelName) {
    refreshTableData(modelName, null, null, true);
}

// Функция для обновления данных таблицы через AJAX.
function refreshTableData(modelName, sortBy = null, sortDirection = null, clearCache = false) {
    if (sortBy == null || sortDirection == null) {
        // Получаем текущие значения сортировки
        let currentSortColumn = $("#dataTable th a.active-sort");
        sortBy = currentSortColumn.data("sort-by");
        sortDirection = currentSortColumn.data("sort-direction");
    }
    
    let table = $("#dataTable tbody");

    $.ajax({
        url: '/' + modelName + '/GetSortedData',
        type: "GET",
        data: {
            sortBy: sortBy,
            sortDirection: sortDirection,
            clearCache: clearCache
        },
        success: function(data) {
            table.html(data);
        },
        error: function(error) {
            console.log(error);
            alert("Произошла ошибка при обновлении данных.");
        }
    });
}

// Функция для обработки события клика на заголовке столбца.
function handleSortClick(modelName, current) {
    // Удаляем классы сортировки у всех столбцов
    $("#dataTable th a").removeClass("active-sort");

    let sortBy = current.data("sort-by");
    let currentSortDirection = current.data("sort-direction");

    // Устанавливаем класс сортировки для текущего столбца
    current.addClass("active-sort");

    // Меняем направление сортировки
    let newSortDirection = currentSortDirection === "asc" ? "desc" : "asc";

    // Обновляем данные таблицы через AJAX
    refreshTableData(modelName, sortBy, newSortDirection);

    // Обновляем значения атрибутов data-sort-direction
    current.data("sort-direction", newSortDirection);
}

// Функция для удаления записи по идентификатору.
function deleteItem(modelName, id) {
    if (confirm("Вы уверены, что хотите удалить эту запись?")) {
        $.ajax({
            url: '/' + modelName + '/DeleteItem',
            type: "POST",
            data: { id: id },
            success: function() {
                refreshTableData(modelName);
                alert("Запись успешно удалена.");
            },
            error: function(error) {
                console.log(error);
                alert("Произошла ошибка при удалении записи.");
            }
        });
    }
}

// Функция для изменения записи по идентификатору.
function editItem(modelName, id) {
    openModal('/' + modelName + '/GetItem', id);
}

// Функция открытия модального окна.
function openModal(url, id) {
    let modal = $('#modalWindow');
    
    $.ajax({
        url: url,
        type: "GET",
        data: { id: id },
        success: function (response) {
            modal.find('.modal-content').html(response);
            modal.modal('show');
        },
        error: function(error) {
            console.log(error);
            alert("Произошла ошибка при загрузке данных.");
        }
    });
}

// Функция для создания новой записи.
function createItem() {
    alert("Вызвано создание записи.");
}

// Функция для экспорта таблицы в Excel.
function exportToExcel() {
    alert("Вызван экспорт в Excel.");
}
