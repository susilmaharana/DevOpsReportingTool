var initialPriorityValue, changedPriorityValue, PriorityType;

$(document).ready(function () {
    $('.priority-edit').on('change', function () {
        initialPriorityValue = $(this).attr('data-val');
        changedPriorityValue = $(this).val();
        if (initialPriorityValue > changedPriorityValue) {
            PriorityType = "Dec";
        }
        else if (initialPriorityValue < changedPriorityValue) {
            PriorityType = "Inc";
        }
    });

    $('#SyncData').click(function () {
        $("#divLoader").show();
        $.ajax({
            url: '/Home/GetProjects/',
            type: 'POST',
            dataType: 'json',
            success: function (response) {
                $("#divLoader").hide();
                alert(response.responseText);
                location.reload();
            },
            error: function (jqXhr, textStatus, errorMessage) {
                $("#divLoader").hide();
                alert(errorMessage);
            }
        });
    });

    $('#getCheckboxValues').click(function () {
        var str = "";
        $.each($("input[name='chkBox']:checked"), function () {
            str = str + "'One\\Business Applications Group Websites\\" + $(this).val() + "',";
        });
        str = str.slice(0, -1);
        $("#divLoader").show();
        $.ajax({
            url: '/Home/GetProjects/',
            type: 'POST',
            dataType: 'json',
            data: { 'SelectedItems': str },
            success: function (response) {
                $("#divLoader").hide();
                alert(response.responseText);
                location.reload();
            },
            error: function (jqXhr, textStatus, errorMessage) {
                $("#divLoader").hide();
                alert(errorMessage);
            }
        });
    });
});

function AddnewComment(a) {
    var Comment = $(a).parent().parent().find(".modal-body #inputComment").val();
    var EditedBy = $(a).parent().parent().find(".modal-body #inputName").val();
    var EditedDate = $(a).parent().parent().find(".modal-body #inputDate").val();
    var NoteID = $(a).parent().parent().find(".modal-body #inputNoteID").val();

    $.ajax({
        url: '/WorkItems/NewNoteUpdate/',
        type: 'POST',
        dataType: 'json',
        data: { 'UpdatedNote': Comment, 'EditedBy': EditedBy, 'EditedDate': EditedDate, 'NoteId': NoteID },
        success: function (response) {
            alert(response.responseText);
            location.reload();
        },
        error: function (jqXhr, textStatus, errorMessage) {
            alert(errorMessage);
        }
    });
}

function AddNewFixVersion(a) {
    var FixVersionName = $(a).parent().parent().find(".modal-body #inputFixVersionName").val();
    var Comment = $(a).parent().parent().find(".modal-body #inputFixVersionComment").val();
    var WorkItemID = $(a).parent().parent().find(".modal-body #inputFixVersionWorkItemID").val();
    var FixVersionID = $(a).parent().parent().find(".modal-body #inputFixVersionID").val();
    $.ajax({
        url: '/WorkItems/AddNewFixVersion/")',
        type: 'POST',
        dataType: 'json',
        data: { 'FixVersionName': FixVersionName, 'Comment': Comment, 'WorkItemID': WorkItemID, 'FixVersionID': FixVersionID },
        success: function (response) {
            alert(response.responseText);
            location.reload();
        },
        error: function (jqXhr, textStatus, errorMessage) {
            alert(errorMessage);
        }
    });
}

function AddnewCommentforNewNote(a) {
    var UpdateBy = $(a).parent().parent().find(".modal-body #inputeditedbyforNewNote").val();
    var Comment = $(a).parent().parent().find(".modal-body #inputCommentforNewNote").val();
    var WorkItemID = $(a).parent().parent().find(".modal-body #inputworkitemforNewNote").val();
    $.ajax({
        url: '/WorkItems/AddNoteforWorkItem/',
        type: 'POST',
        dataType: 'json',
        data: { 'Comment': Comment, 'WorkItemID': WorkItemID, 'UpdateBy': UpdateBy },
        success: function (response) {
            alert(response.responseText);
            location.reload();
        },
        error: function (jqXhr, textStatus, errorMessage) {
            alert(errorMessage);
        }
    });
}

function deleteComment(a) {
    var NoteId = $(a).parent().parent().find(".modal-body #DeleteNoteID").val();
    $.ajax({
        url: '/WorkItems/DeleteNote/',
        type: 'POST',
        dataType: 'json',
        data: { 'id': NoteId },
        success: function (response) {
            alert(response.responseText);
            location.reload();
        },
        error: function (jqXhr, textStatus, errorMessage) {
            alert(errorMessage);
        }
    });
}

function UpdateWorkItem(a) {
    var devopsid = $(a).parent().parent().find(".modal-body #DevopsItem").val();
    var summary = $(a).parent().parent().find(".modal-body #Summary").val();
    var priority = $(a).parent().parent().find(".modal-body #Priority").val();
    var fixversion = $(a).parent().parent().find(".modal-body #FixVersion").val();
    var pendingwith = $(a).parent().parent().find(".modal-body #PendingWith").val();
    var date = $(a).parent().parent().find(".modal-body #date").val();
    var notes = $(a).parent().parent().find(".modal-body #Notes").val();
    var status = $(a).parent().parent().find(".modal-body #Status").val();
    var assignee = $(a).parent().parent().find(".modal-body #Assignee").val();
    $.ajax({
        url: '/WorkItems/UpdateWorkitembyID/',
        type: 'POST',
        dataType: 'json',
        data: {
            'devopsid': devopsid,
            'summary': summary,
            'priority': priority,
            'fixversion': fixversion,
            'pendingwith': pendingwith,
            'date': date,
            'notes': notes,
            'status': status,
            'assignee': assignee
        },
        success: function (response) {
            alert(response.responseText);
            location.reload();
        },
        error: function (jqXhr, textStatus, errorMessage) {
            alert(errorMessage);
        }
    });
}

$(function () {
    $('.edit-mode').hide();
    $('.edit-user, .cancel-user').on('click', function () {
        var tr = $(this).parents('tr:first');
        tr.find('.edit-mode, .display-mode').toggle();
    });

    $('.save-user').on('click', function () {
        var txt;
        var tr = $(this).parents('tr:first');
        var WIStatus = tr.find("#Status" + $(this).attr("id")).val();
        if (WIStatus === 1) {
            var result = confirm("Status Changed to 'Done' for selected workItem! Are you sure you want to change the Priorities Accordingly ?");
            if (result === true) {
                txt = "OK";
            } else {
                txt = "Cancel";
            }
        }

        if (initialPriorityValue === undefined) {
            initialPriorityValue = 0;
        }
        if (changedPriorityValue === undefined) {
            changedPriorityValue = 0;
        }

        var Summary = tr.find("#Summary").val();
        var Priority = tr.find("#Priority").val();
        var pendingWith = tr.find("#PendingWith").val();
        var ReleaseDate = tr.find("#ExpectedReleaseDate").val();
        var Status = tr.find("#Status" + $(this).attr("id")).val();
        var Assignee = tr.find("#Assignee" + $(this).attr("id")).val();
        var DevopsID = tr.find("#DevopsItemID a").html();

        tr.find('.edit-mode, .display-mode').toggle();
        var UserModel =
        {
            "DevopsItemID": DevopsID,
            "Summary": Summary,
            "Priority": Priority,
            "pendingWith": pendingWith,
            "ExpectedReleaseDate": ReleaseDate,
            "Status": Status,
            "Assignee": Assignee
        };

        $.ajax({
            url: '/WorkItems/UpdateRecord/',
            data: { 'initialPriorityValue': initialPriorityValue, 'changedPriorityValue': changedPriorityValue, 'PriorityType': PriorityType, 'PromptClick': txt, 'workitem': JSON.stringify(UserModel) },
            type: 'POST',
            dataType: 'json',
            success: function (response) {
                alert(response.responseText);
                location.reload();
            },
            error: function (jqXhr, textStatus, errorMessage) {
                alert(errorMessage);
            }
        });
    });
})