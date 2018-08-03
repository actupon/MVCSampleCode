/*
Created By : Maulik Joshi
Created On : 15/10/2016
Purpose : Topic related JS file.
*/

var lnkEdit = '#lnkEdit';

/*
Created By : Maulik Joshi
Created On : 14th Oct, 2016
Purpose : Create a new topic.
*/
function CreateTopic(engagementId) {
    var url = '/Dashboard/CreateConversation/';
    $.blockUI({
        message: $('#LoadingMsg'),
        css: { width: '20%' }
    });
    $.post(url, { engagementId: engagementId }, function (data) {
        $.unblockUI();
        RenderNewTopics("New Conversation", data);
    }, "json");
}

/*
Created By : Maulik Joshi
Created On : 30th Nov, 2016
Purpose : It will render a new list item as new topic and clear older conversion. Added reusability.
*/
function RenderNewTopics(topicName, data) {
    $.ajax({
        url: renderNewTopicURL,
        type: 'GET',
        dataType: 'html',
        data: {
            Id: data.TopicId,
            EngagementId: data.EngagementId,
            ConversationId: data.Id,
            Title: topicName
        },
        success: function (result) {
            $(".contant-section-left02 ul").append(result);
            $("#f-option_" + data.TopicId).prop("checked", true);
            renderMessage(null);
        },
        error: function (e) {
            console.log(e.message);
        }
    });
}

/*
    Created By : Maulik Joshi
    Created On : 15th Oct, 2016
    Purpose : Load conversation from database using ConversationId.
*/
function LoadConversation(id)
{
    $.ajax({
        url: loadConversationURL +'/?conversationId=' + id,
        type: 'post',
        beforeSend: function()
        {
            $.blockUI({
                message: $('#LoadingMsg'),
                css: { width: '20%' }
            });
        },
        success: function (item) {
            if(item.Messages != null)
            {
                ClearChatArea();
                $.each(item.Messages, function (item, message) {
                    renderMessage(message);
                });
            }
            else
            {
                ClearChatArea();
            }
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
   Created On : 16th Oct, 2016
   Purpose : Clear / Blank Chat Area.
*/
function ClearChatArea()
{
    try
    {
        $("#chatArea").empty();
    }
    catch(e)
    {
        consol.log(e.message);
    }
}

/*
Created By : Maulik Joshi
Created On : 15/10/2016
Purpose : To show edit topic icon.
*/
function ShowEditIcon(topicId) {
    $(lnkEdit + topicId).show();
}

/*
Created By : Maulik Joshi
Created On : 15/10/2016
Purpose : To show topic textbox in edit mode.
*/
function ShowEditTextbox(topicId) {
    $('#' + topicId).show().css("display", "block").focus();
    $('#' + topicId).val($.trim($('#' + topicId).val()));
    $('#lblTopic' + topicId).hide();
    $(lnkEdit + topicId).parent().parent().removeClass();
}

/*
Created By : Maulik Joshi
Created On : 17/10/2016
Purpose : Handle on key press event of topic textbox.
*/
function HandleOnKeyPress(e, topicId) {
    var keyCode = e.keyCode;
    if (keyCode == 13) { // Enter key
        return SaveTopic(topicId)
    }
    else {
        return IsAlphaNumeric(e); // To check alpha numeric value.
    }
}

/*
    Created By : Maulik Joshi
    Created On : 15/10/2016
    Purpose : Update topic.
*/
function SaveTopic(topicId) {
    var returnValue = false;
    var title = $.trim($("#" + topicId).val());
    if (ValidateTopic(topicId)) { // Validate topic
        var engagementId = GetParameterValues('engagementId');
        $.ajax({
            type: 'POST',
            contentType: 'application/json; charset=utf-8',
            dataType: 'JSON',
            url: updateConversationURL,
            data: "{'engagementId': " + engagementId + ",'topicId': " + topicId + ",'title': '" + title + "'}",
            success: function (item) {
                {
                    $('#' + topicId).hide(); // Show text box value in label.
                    $('#lblTopic' + topicId).show().text(title);
                    $(lnkEdit + topicId).parent().parent().addClass("listTopic");
                }
            },
            error: function (e) {
                console.log(e.message);
                $(lnkEdit + topicId).parent().parent().addClass("listTopic");
            }
        });
        returnValue = true;
    }
    return returnValue;
}

/*
Created By : Maulik Joshi
Created On : 16/10/2016
Purpose : Validate update topic details.
*/
function ValidateTopic(topicId) {
    var returnValue = false;
    var topic = $.trim($("#" + topicId).val());
    if (topic.length <= 2) {
        $("#" + topicId).focus();
        alert(conversationInputValidation);
    }
    else {
        returnValue = true;
    }
    return returnValue;
}