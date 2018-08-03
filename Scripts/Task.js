/// Created By : Maulik Joshi
/// Created On : 19th November, 2016
/// Purpose : Task JS file
function InitializeTaskQuill() {
    quill = new Quill('#editor-container_task', {
    modules: {
        formula: true,
        toolbar: '#toolbar-container_task'
    },
    placeholder: composeTask,
    theme: 'snow'
});

quill.on('text-change', function (delta) { $('#editor-container_task_hidden').val(JSON.stringify(quill.getContents())); });

if ($("#editor-container_task_instructions").length > 0) {
     quill_instructions = new Quill('#editor-container_task_instructions', {
        theme: 'snow',
        placeholder: composeSetOfInstructions,
        modules: {
            toolbar: [[{ 'list': 'ordered' }, { 'list': 'bullet' }]]  // Include button in toolbar
        },
    });
    
    quill_instructions.on('text-change', function (delta) { $('#editor-container_task_hidden_instructions').val(JSON.stringify(quill_instructions.getContents())); });
}
}

/// Created By : Maulik Joshi
/// Created On : 20th November, 2016
/// Purpose : Show/Hide task fields as per task type
function ShowHideTaskField() {
    var taskSource = "#TaskType";
    var taskType = $(taskSource).val();

    if (taskType.toLowerCase() == assignment.toLowerCase()) {
        $("#YouTubeLink").val('');
        $("#divPointLanuageLabel").show();
        $("#divPointLanuageField").show();
        $("#divInstructions").show();
        $("#divTypeOfExpectedAns").show();
        $("#divYouTube").hide();
    }
    else {
        $("#XPPoints").val(0);
        if ($("#editor-container_task_instructions").length > 0) {
            quill_instructions.setText('');
        }
        $("#divPointLanuageLabel").hide();
        $("#divPointLanuageField").hide();
        $("#divInstructions").hide();
        $("#divTypeOfExpectedAns").hide();
        $("#divYouTube").show();
    }
}

/// Created By : Maulik Joshi
/// Created On : 20th November, 2016
/// Purpose : Validate task fields.
function ValidateTask() {
    DisplayPopup();
    var validateName = false;

    if ($.trim($("#Name").val()) === "") {
        $("#divName").addClass("has-error2");
    } else {
        $("#divName").removeClass("has-error2");
        validateName = true;
    }

    if (validateName) {
        return true;
    } else {
        UnBlockUI();
        return false;
    }
}

/// Created By : Maulik Joshi
/// Created On : 22nd November, 2016
/// Purpose : Save task success call.
function ajaxSuccessTask(response) {
    UnBlockUI();
    if (response.success) {
        if (response.isAdd) {
            // Add Task Node - New
            $('#divJsTree').jstree().create_node(response.SectionId, CreateNode(response.TaskId, response.Title, "Task"), "last");
            // Open newly created node / section by default so Volunteer can view task titles.
            $('#divJsTree').jstree().open_node(response.SectionId);
        }
        else {
            // Edit Task Node - Rename
            $('#divJsTree').jstree().rename_node(response.TaskId, response.Title);
        }
        $('#taskModal').modal('hide');
    }
    else {
        console.log(errorCreatingTask);
    }
}