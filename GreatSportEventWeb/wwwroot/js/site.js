// Функция для обновления таблицы с очисткой кэша.
function refreshCache(controllerName) {
    refreshTableData(controllerName, null, null, true);
    alert("Таблица успешно обновлена.");
}

// Функция для обновления данных таблицы через AJAX.
function refreshTableData(controllerName, sortBy = null, sortDirection = null, clearCache = false) {
    if (sortBy == null || sortDirection == null) {
        // Получаем текущие значения сортировки
        let currentSortColumn = $("#dataTable th a.active-sort");
        sortBy = currentSortColumn.data("sort-by");
        sortDirection = currentSortColumn.data("sort-direction");
    }

    let table = $("#dataTable tbody");

    $.ajax({
        url: '/' + controllerName + '/GetSortedData',
        type: "GET",
        data: {
            sortBy: sortBy,
            sortDirection: sortDirection,
            clearCache: clearCache
        },
        success: function (data) {
            table.html(data);
        },
        error: function (error) {
            console.log(error);
            alert("Произошла ошибка при обновлении данных.");
        }
    });

    $("#searchInput").val("");
}

// Функция для обработки события клика на заголовке столбца.
function handleSortClick(controllerName, current) {
    // Удаляем классы сортировки у всех столбцов
    $("#dataTable th a").removeClass("active-sort");

    let sortBy = current.data("sort-by");
    let currentSortDirection = current.data("sort-direction");

    // Устанавливаем класс сортировки для текущего столбца
    current.addClass("active-sort");

    // Меняем направление сортировки
    let newSortDirection = currentSortDirection === "asc" ? "desc" : "asc";

    // Обновляем данные таблицы через AJAX
    refreshTableData(controllerName, sortBy, newSortDirection);

    // Обновляем значения атрибутов data-sort-direction
    current.data("sort-direction", newSortDirection);
}

// Функция получения словаря данных по id.
function getDictionaryFromId(id) {
    let data = null;

    if (typeof id === 'object') {
        // Если id является объектом (словарем)
        data = id;
    } else if (typeof id === 'string') {
        // Если id является строкой
        data = {id: id};
    } else {
        // Если тип id не соответствует ожидаемым вариантам, обработать эту ситуацию и выдать ошибку
        console.error('Неверный тип id:', typeof id);
        alert("Произошла ошибка при получении id.");
    }
    
    return data;
}

// Функция для удаления записи по идентификатору.
function deleteItem(controllerName, id) {
    let data = getDictionaryFromId(id);
    
    if (data != null) {
        if (confirm("Вы уверены, что хотите удалить эту запись?")) {
            $.ajax({
                url: '/' + controllerName + '/DeleteItem',
                type: "POST",
                data: data,
                success: function () {
                    refreshTableData(controllerName);
                    alert("Запись успешно удалена.");
                },
                error: function (error) {
                    console.log(error);
                    alert("Произошла ошибка при удалении записи.");
                }
            });
        }
    }
}

// Функция для изменения записи по идентификатору.
function editItem(controllerName, id) {
    openModal('/' + controllerName + '/GetItem', id);
}

// Функция открытия модального окна.
function openModal(url, id) {
    let data = getDictionaryFromId(id);

    if (data != null) {
        // Получаем объект модального окна
        let modal = new bootstrap.Modal(document.getElementById('modalWindow'));

        $.ajax({
            url: url,
            type: "GET",
            data: data,
            success: function (response) {
                $('#modalWindow').find('.modal-content').html(response);
                modal.show();
            },
            error: function (error) {
                console.log(error);
                alert("Произошла ошибка при загрузке данных.");
            }
        });
    }
}

// Функция для создания новой записи.
function createItem(controllerName) {
    openModal('/' + controllerName + '/CreateItem', '-1');
}

// Функция для поиска в таблице
function filterTable() {
    let searchText = $("#searchInput").val().trim().toLowerCase();
    let rows = $("#dataTable tbody tr");

    rows.each(function () {
        let row = $(this);
        let showRow = false;

        row.find("td").each(function () {
            let cellText = $(this).text().toLowerCase();
            if (cellText.includes(searchText)) {
                showRow = true;
                return false; // Exit the inner loop if a match is found in any cell
            }
        });

        if (showRow) {
            row.show();
        } else {
            row.hide();
        }
    });
}

// Функция для очистки поля поиска.
function clearInputFilter() {
    $("#searchInput").val("").focus();
    filterTable();
}

// Функция для проверки данных на уникальность.
function checkUnique(controllerName, excludedFields = []) {
    let fieldTags = ['input', 'textarea', 'select'];
    let formData = {};

    fieldTags.forEach(function (tag) {
        $('form ' + tag).each(function () {
            let fieldName = $(this).attr('name');

            // Проверяем, что имя поля не содержится в исключенных полях
            if (!excludedFields.includes(fieldName)) {
                formData[fieldName] = $(this).val();
            }
        });
    });

    let formAlert = $('#formAlert');

    $.ajax({
        url: '/' + controllerName + '/CheckUnique',
        type: 'POST',
        data: JSON.stringify(formData),
        contentType: 'application/json',
        success: function (result) {
            if (result.isUnique && result.isValid) {
                formAlert.hide();
                $('form').unbind('submit').submit();
            } else if (!result.isValid) {
                formAlert.text('Поля неправильно заполнены!');
                formAlert.show();
            } else if (!result.isUnique) {
                formAlert.text('Запись с такими данными уже существует!');
                formAlert.show();
            }
        }
    });
}
