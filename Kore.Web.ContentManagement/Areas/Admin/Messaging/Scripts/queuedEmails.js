﻿'use strict'

function deleteRecord(id) {
    if (confirm(translations.DeleteRecordConfirm)) {
        $.ajax({
            url: "/odata/kore/cms/QueuedEmails(guid'" + id + "')",
            type: "DELETE",
            async: false
        })
        .done(function (json) {
            $('#Grid').data('kendoGrid').dataSource.read();
            $('#Grid').data('kendoGrid').refresh();

            $.notify(translations.DeleteRecordSuccess, "success");
        })
        .fail(function () {
            $.notify(translations.DeleteRecordError, "error");
        });
    }
};

$(document).ready(function () {

    $("#Grid").kendoGrid({
        data: null,
        dataSource: {
            type: "odata",
            transport: {
                read: {
                    url: "/odata/kore/cms/QueuedEmails",
                    dataType: "json"
                }
            },
            schema: {
                data: function (data) {
                    return data.value;
                },
                total: function (data) {
                    return data["odata.count"];
                },
                model: {
                    id: "Id",
                    fields: {
                        Subject: { type: "string" },
                        ToAddress: { type: "string" },
                        CreatedOnUtc: { type: "date" },
                        SentOnUtc: { type: "date" },
                        SentTries: { type: "number" }
                    }
                }
            },
            pageSize: 10,
            serverPaging: true,
            serverFiltering: true,
            serverSorting: true,
            sort: { field: "CreatedOnUtc", dir: "desc" }
        },
        filterable: true,
        sortable: {
            allowUnsort: false
        },
        pageable: {
            refresh: true
        },
        scrollable: false,
        columns: [{
            field: "Subject",
            filterable: true
        }, {
            field: "ToAddress",
            title: "To Address",
            filterable: true
        }, {
            field: "CreatedOnUtc",
            title: "Created On (UTC)",
            filterable: true
        }, {
            field: "SentOnUtc",
            title: "Sent On (UTC)",
            filterable: true
        }, {
            field: "SentTries",
            title: "Sent Tries",
            filterable: true
        }, {
            field: "Id",
            title: " ",
            template:
                '<div class="btn-group"><a onclick="deleteRecord(\'#=Id#\')" class="btn btn-danger btn-xs">' + translations.Delete + '</a>' +
                '</div>',
            attributes: { "class": "text-center" },
            filterable: false,
            width: 100
        }]
    });
});