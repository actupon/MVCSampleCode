/*
Created By : Maulik Joshi
Created On : 24th November, 2016
Purpose : Intialize TaskModel - Popup.
*/
function InitializeTaskPopUp() {
    $("#taskModal").on('shown.bs.modal', function (e) {
        GetTaskTypeView(e);
    });
}

/*
Created By : Maulik Joshi
Created On : 19th November, 2016
Purpose : Get view as per task type.
*/
function GetTaskTypeView(e) {
    var lessonId = $("#hdnLessonId").val();
    var sectionId = $("#hdnSectionId").val();
    var taskId = $("#hdnTaskId").val();
    var heading = '';

    var url = getTaskTypeViewPath;
    url = url + "?lessonId=" + lessonId + "&sectionId=" + sectionId + "&taskId=" + taskId;
    $.ajax({
        type: 'GET',
        contentType: 'application/json; charset=utf-8',
        cache: false,
        url: url,
        beforeSend: DisplayPopup(),
        success: function (data, status, xhr) {
            if (xhr.status == statusCode) {
                $("#divTaskBody").html(data);
                 $("#Name").focus();
                if ($.trim(taskId) === '') {
                    heading = addNewTask;
                    $("#TaskType").removeAttr("disabled");
                }
                else {
                    heading = editTask;
                    $("#TaskType").attr("disabled", "disabled");
                    if ($.trim($('#editor-container_task_hidden').val()) !== "") {
                        quill.setContents(JSON.parse($('#editor-container_task_hidden').val()));
                    }

                    var taskType = $("#TaskType").val();
                    if (taskType.toLowerCase() == assignment.toLowerCase()) {
                        if ($.trim($('#editor-container_task_hidden_instructions').val()) !== "") {
                            quill_instructions.setContents(JSON.parse($('#editor-container_task_hidden_instructions').val()));
                        }
                    }
                }
                $("#tasktitle").text(heading);
                ShowHideTaskField();
                UnBlockUI();
            }
        },
        error: function (xhr, status, error) {
            UnBlockUI();
            console.log('Error : ' + xhr.responseText);
        }
    });
}

/*
Created By : Maulik Joshi
Created On : 20th November, 2016
Purpose : Contextmenu defination.
*/
function customMenu(node) {
    var items = {
        'AddTask': {
            'label': addTask,
            'action': function () {
                $("#hdnSectionId").val(node.id);
                $("#hdnTaskId").val('');
                $("#taskModal").modal('show');
            }
        },
        'EditTask': {
            'label': editTask,
            'action': function () {
                $("#hdnSectionId").val(node.parent);
                $("#hdnTaskId").val(node.id);
                $("#taskModal").modal('show');
            }
        },
        'DeleteTask': {
            'label': deleteTask,
            'action': function () {
                bootbox.confirm('<strong>' + deleteTaskConfirmation + '</strong>', function (result) {
                    if (result) {
                        var lessonId = $("#hdnLessonId").val();
                        var sectionId = node.parent;
                        var taskId = node.id;
                        DeleteTask(lessonId, sectionId, taskId)
                    }
                });
            }
        },
        'DeleteSection': {
            'label': deleteSection,
            'action': function () {
                bootbox.confirm('<strong>' + deleteSectionConfirmation + '</strong>', function (result) {
                    if (result) {
                        var lessonId = $("#hdnLessonId").val();
                        var sectionId = node.id;
                        DeleteSection(lessonId, sectionId)
                    }
                });
            }
        }
    }

    if (node.parent === '#') {
        delete items.EditTask;
        delete items.DeleteTask;
    } else {
        delete items.AddTask;
        delete items.DeleteSection;
    }
    return items;
}

/*
Created By : Maulik Joshi
Created On : 20th November, 2016
Purpose : Add Section to jsTree dynamically.
*/
function AddSection(lessonId, title) {
    var isAddSection = false;
    var isAdded = $("#isAdded").val();
    $.ajax({
        cache: false,
        type: "POST",
        url: addSectionURL,
        dataType: 'json',
        data: { "lessonId": lessonId, "title": title, "isAdded": isAdded },
        beforeSend: DisplayPopup(),
        success: function (data, status, xhr) {
            if (xhr.status === statusCode) {
                if (!$('#divJsTree').jstree().get_container().hasClass("divJsTree"))
                    $('#divJsTree').jstree().get_container().addClass("divJsTree").show();

                $('#divJsTree').jstree().create_node('#', CreateNode(data.SectionId, title, "Section"), "last");

                $('#divJsTree').removeClass("hide");
            }
            $("#Sections").val('');
            $.unblockUI();
        },
        error: function (e) {
            $.unblockUI();
            console.log(e.message);
        }
    });
}

/*
Created By : Maulik Joshi
Created On : 22nd November, 2016
Purpose : Delete Section Item along with tasks.
*/
function DeleteSection(lessonId, sectionId) {
    $.ajax({
        cache: false,
        type: "POST",
        url: deleteSectionURL,
        data: { "lessonId": lessonId, "sectionId": sectionId },
        beforeSend: DisplayPopup(),
        success: function (data, status, xhr) {
            if (xhr.status == statusCode) {
                $('#divJsTree').jstree().delete_node(sectionId);
                HideSectionDIV();
            }
            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            console.log(e.message);
            $.unblockUI();
        }
    });
}

/*
Created By : Maulik Joshi
Created On : 29nd November, 2016
Purpose : Delete Task.
*/
function DeleteTask(lessonId, sectionId, taskId) {
    $.ajax({
        cache: false,
        type: "POST",
        url: deleteTaskURL,
        data: { "lessonId": lessonId, "sectionId": sectionId, "taskId": taskId },
        beforeSend: DisplayPopup(),
        success: function (data, status, xhr) {
            if (xhr.status == statusCode) {
                $('#divJsTree').jstree().delete_node(taskId);
                HideSectionDIV();
            }
            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            console.log(e.message);
            $.unblockUI();
        }
    });

}

/*
Created By : Maulik Joshi
Created On : 20th November, 2016
Purpose : To create Section / Task node dynamically.
*/
function CreateNode(id, title, type) {
    var node = {
        'id': '' + id + '',
        'text': '' + title + '',
        "icon": type === "Section" ? taskIconPath : 'jstree-icon jstree-file'
    }
    return node;
}

/*
Created By : Maulik Joshi
Created On : 23rd November, 2016
Purpose : Hide section DIV if no section exist.
*/
function HideSectionDIV() {
    if ($("#divJsTree").jstree(true).get_children_dom($('#divJsTree')).length == 0)
        $("#divJsTree").jstree(true).get_container().removeClass("divJsTree").hide();
}

/*
Created By : Maulik Joshi
Created On : 23rd November, 2016
Purpose : Dropdown Source change event.
*/
function SourceChange(selectedItem) {
    if (selectedItem.toUpperCase() === samplecode.toUpperCase()) {
        ShowHideDivBasedOnSource(true);
        $("#LessonTitle").parent().removeClass("has-error2");
        $("#editor-container_lesson").removeClass("editor-container_lesson_color");
    }
    else {
        ShowHideDivBasedOnSource(false);
        $("#SourceUrl").parent().removeClass("has-error2");
    }
}

/*
Created By : Maulik Joshi
Created On : 23rd November, 2016
Purpose : Change lesson category dropdown.
*/
function LessonCategory() {
    $("#ddlLessonCategory").change(function () {
        var selectedItem = $(this).val();;
        var ddlLessonSubject = $("#ddlLessonSubject");

        $.ajax({
            cache: false,
            type: "GET",
            url: ddlLessonCategory,
            data: { "category": selectedItem },
            success: function (data, status, xhr) {
                if (xhr.status == statusCode) {
                    ddlLessonSubject.html('');
                    $.each(data, function (id, option) {
                        ddlLessonSubject.append($('<option></option>').val($.trim(option.name)).html($.trim(option.name)));
                    });
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                console.log(lessonFailuer);
                statesProgress.hide();
            }
        });
    });
}

/*
Created By : Maulik Joshi
Created On : 23rd November, 2016
Purpose : Add Section.
*/
function InitializeSection() {
    if (ValidateSection()) {
        var lessonId = $("#hdnLessonId").val();
        var title = $.trim($("#Sections").val());
        AddSection(lessonId, title);
    }
}

/*
Created By : Maulik Joshi
Created On : 24th November, 2016
Purpose : Intialize jsTree.
*/
function InitializeJsTree() {
    $('#divJsTree').jstree({
        "plugins": ["contextmenu"],
        "contextmenu": {
            "items": customMenu
        },
        "core": {
            "check_callback": true
        }
    }).on('ready.jstree', function (e, data) {

        var node = $(this).jstree(true).get_json('#', { 'flat': true });

        if (node.length > 0 && !$('#divJsTree').jstree().get_container().hasClass("divJsTree")) {
            $('#divJsTree').jstree().get_container().removeClass("hide").addClass("divJsTree");
        }
    });
}

/*
Created By : Maulik Joshi
Created On : 24th November, 2016
Purpose : Intialize Quill.
*/
function InitializeQuill() {
    var quill = new Quill('#editor-container_lesson', {
        modules: {
            formula: true,
            toolbar: '#toolbar-container_lesson'
        },
        placeholder: quillInstrOverview,
        theme: 'snow'
    });

    quill.on('text-change', function (delta) {
        $('#editor-container_lesson_hidden').val(JSON.stringify(quill.getContents()));
    });

    if ($.trim($('#editor-container_lesson_hidden').val()) !== "") {
        quill.setContents(JSON.parse($('#editor-container_lesson_hidden').val()));
    }
}

/// Created By : Maulik Joshi
/// Created On : 24th November, 2016
/// Purpose : Show/Hide Div based on Source type.
function ShowHideDivBasedOnSource(IsSample) {
    //change  button text based on isAdded flag
    var isAdded = $("#isAdded").val();
    if (isAdded.toUpperCase() == "FALSE") {
        $("#Save").attr('value', update);
        $("#ui-id-5").html(editLesson);
        $("#Sections").val('');
    }
    else {
        $("#Save").attr('value', save);
        $("#ui-id-5").html(createLesson);
    }

    //publish button disabled when admin logged in.
    if (IsAdmin.toUpperCase() == "TRUE" && isAdded.toUpperCase() == "FALSE") {
        if ($("#Status").val().toUpperCase() == draftStatus.toUpperCase()) {
            $("#btnPublished").prop("disabled", true);
        }
    }

    //Publish button and Save button enabled when volunteer logged in
    if (IsVolunteer == true && isAdded.toUpperCase() == "FALSE") {
        $("#btnPublished").prop("disabled", false);
    }

    if ($("#Source").val().toUpperCase() != Sample.toUpperCase()) {
        IsSample = false;
    }

    if (IsSample == true) {
        $('#dvSample').show();
        $('#dvUrl').hide();
    }
    else {
        $('#dvSample').hide();
        $('#dvUrl').show();
    }
}

/// Created By : Maulik Joshi
/// Created On : 24th November, 2016
/// Purpose : Validate Lesson fields.
function ValidateLesson(btnType) {
    $("#hdnButtonType").val(btnType);
    DisplayPopup();

    var validateLesson = false;
    if ($("#Source").val().toUpperCase() == Sample.toUpperCase()) {

        if ($.trim($("#LessonTitle").val()) === "") {
            $("#LessonTitle").parent().addClass("has-error2");
        } else {
            $("#LessonTitle").parent().removeClass("has-error2");
            validateLesson = true;
        }

        if ($("#editor-container_lesson").text() === "") {
            if ($.trim($("#editor-container_lesson").val()) === "") {
                $("#editor-container_lesson").addClass("editor-container_lesson_color");
                validateLesson = false;
            } else {
                $("#editor-container_lesson").removeClass("editor-container_lesson_color");
                if (validateLesson) {
                    validateLesson = true;
                }
            }
        }
        else {
            $("#editor-container_lesson").removeClass("editor-container_lesson_color");
        }
    }
    else {
        if ($.trim($("#SourceUrl").val()) === "") {
            $("#SourceUrl").parent().addClass("has-error2");
        } else {
            $("#SourceUrl").parent().removeClass("has-error2");
            validateLesson = true;
        }
    }

    if (validateLesson == false) {
        UnBlockUI();
    }
    return validateLesson;
}

/// Created By : Maulik Joshi
/// Created On : 02th December, 2016
/// Purpose : Submit Lesson with Draft Mode.
function SubmitLessonWithDraftMode() {
    var isValidate = ValidateLesson();
    var isAdded = $("#isAdded").val();
    var activetab = $("#activeTab").val();
    var lessonSuccessMsg = lessonSuccess;
    if (isAdded.toUpperCase() == "FALSE") {
        lessonSuccessMsg = lessonEditedSuccess;
        activetab = tab;
    }
    if (isValidate == true) {
        $("#form0").attr("action", "/Lesson/CreateDraftLesson").submit();
        $.unblockUI();
        bootbox.alert('<strong>' + lessonSuccessMsg + '</strong>', function (result) {
            ShowHideDivBasedOnSource(true);
            $('#btnPublished').prop('disabled', 'disabled');
            window.location.href = "Dashboard?Selected=" + myLessontab;
        });
    }
    else {
        $.unblockUI();
    }
}

/// Created By : Maulik Joshi
/// Created On : 02th December, 2016
/// Purpose : Published Lesson.
function SaveLesson() {
    var isValidate = ValidateLesson();
    var isAdded = $("#isAdded").val();
    var activetab = $("#activeTab").val();

    var lessonPublishMsg = lessonPublish;
    if (isAdded.toUpperCase() == "FALSE") {
        lessonPublishMsg = lessonPublishSuccessInEdited;
        activetab = tab;
    }

    if (isValidate == true) {
        $("#form0").attr("action", "/Lesson/SaveLesson").submit();
        $.unblockUI();
        bootbox.alert('<strong>' + lessonPublishMsg + '</strong>', function (result) {
            ShowHideDivBasedOnSource(true);
            $('#btnPublished').prop('disabled', 'disabled');
            window.location.href = "Dashboard?Selected=" + myLessontab;
        });
    }
    else {
        $.unblockUI();
    }
}

/// Created By : Maulik Joshi
/// Created On : 20th November, 2016
/// Purpose : Validate Section fields.
function ValidateSection() {
    var validateSection = false;
    UnBlockUI();
    if ($.trim($("#Sections").val()) === "") {
        $("#Sections").parent().addClass("has-error2");
    } else {
        $("#Sections").parent().removeClass("has-error2");
        validateSection = true;
    }

    if (validateSection == false) {
        UnBlockUI();
        validateSection = false;
    }
    return validateSection;
}

/// Created By : Maulik Joshi
/// Created On : 05th December, 2016
/// Purpose : Delete lesson confirmation.
function DeleteLessonConfirmation(lessonId) {
    bootbox.confirm('<strong>' + deleteLessonConfirmation + '</strong>', function (result) {
        if (result) {
            DeleteLesson(lessonId);
        }
    });
}

/// Created By : Maulik Joshi
/// Created On : 05th December, 2016
/// Purpose : Delete lesson.
function DeleteLesson(lessonId) {
    $.ajax({
        cache: false,
        type: "POST",
        url: deleteLessonURL,
        data: { "lessonId": lessonId },
        beforeSend: DisplayPopup(),
        success: function (data) {
            if (data.Message === success) {
                bootbox.alert('<strong>' + lessonDeleteSuccess + '</strong>', function (result) {
                    window.location.href = "Dashboard?Selected=" + myLessontab;
                });
            }
            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            console.log(e.message);
            $.unblockUI();
        }
    });
}

/// Created By : Maulik Joshi
/// Created On : 05th December, 2016
/// Purpose : Get My Lesson.
function GetMyLesson() {
    $.ajax({
        cache: false,
        type: "GET",
        url: myLessonURL,
        beforeSend: DisplayPopup(),
        success: function (data) {
            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            console.log(e.message);
            $.unblockUI();
        }
    });
}

/// Created By : Maulik Joshi
/// Created On : 05th December, 2016
/// Purpose : Redirect to add new lesson.
function CreateNewLesson() {
    window.location.href = createNewLessonURL;
}