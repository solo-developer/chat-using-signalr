var chat = undefined;

$(document).ready(function () {
    chat = $.connection.chatHub;
    chat.client.broadcastMessage = function (name, message, message_from, message_to) {
        var encodedName = $('<div/>').text(name).html();
        var encodedMsg = $('<div/>').text(message).html();

        //check if the message is to be shown in a tab that is already created
        //if tab is already created just prepend message in that tab else add a new tab

        if ($('#' + message_from).length == 0 && $('#' + message_to).length == 0) {
            if (message_from == $('#displayname').val()) {
                addTabs(message_from);
                appendMessage(message_to, encodedName, encodedMsg, 'left');
            }
            else if (message_to == $('#displayname').val()) {
                addTabs(message_from);
                appendMessage(message_from, encodedName, encodedMsg, 'left');
            }

        }
        else {
            //add only messages since tab is already present

            if (message_to == $('#displayname').val()) {
                appendMessage(message_from, encodedName, encodedMsg, 'left');
                //if tab is not active then add a count in tab
                if ($('div.active').eq(0).attr('id') != message_from) {
                    $('#' + message_from + '_User').text(message_from + '(1)');
                }
            }

            if (message_from == $('#displayname').val() && message_to != "All") {
                appendMessage(message_to, encodedName, encodedMsg, 'right', 'right');
            }
            if (message_to == "All") {
                let direction = message_from == $('#displayname').val() ? 'right' : 'left';
                appendMessage('All', encodedName, encodedMsg, direction, direction);

                if (message_from != $('#displayname').val()) {
                    if ($('.active').eq(0).attr('id') != message_to) {
                        $('#' + message_to + '_User').text(message_to + '(1)');
                    }
                }
            }
        }
    };
    //append just added username to the active users list
    chat.client.getUsername = function (name) {

        if ($('#displayname').val() != name) {

            $('#active_users').append("<div class='form-group active_us'>                 <p class='form-control text-primary' >" + name + "</p></div>");
            $('.active_us').on('click', function () {

                //store value in variable
                var txt = $(this).find('p').text();
                //put value in hidden box
                $('#message_to').val(txt);
                $('#message').focus();
                if ($('#' + txt).length == 0) {
                    $('.nav-tabs').append('<li><a data-toggle="tab" href="#' + txt + '" class="a-white" id="' + txt + '_User" >' + txt + '</a></li>');
                    //add new div-content and prepend a message
                    $('.tab-content').prepend('<div id="' + txt + '" class="tab-pane fade"><div class="container-fluid" id="discussion_' + txt + '"></div></div>');
                }
                $("#" + txt + "_User").click();
            });
        };
    };

    //remove user from list whenever they leaves
    chat.client.removedUser = function (user) {

        if ($('#displayname').val() != user) {
            //get count
            var count = $('.active_us').length;
            var active_us = $('.active_us');
            for (var i = 0; i < count; i++) {
                if (jQuery.trim(active_us.eq(i).text()) == user) {
                    $(active_us.eq(i)).remove();
                }
            }
        };
    };
    chat.client.isUserAllowed = function (is_allowed) {
        if (is_allowed == false) {
            alert("The username you entered is already used.Please give another name");
            location.reload(true);
        }
    }

    //update name whenever user others change their name
    chat.client.changedUsername = function (original_user, new_username) {
        var active_users_count = $('.active_us').length;

        var active_user_name = $('div.active').eq(0).attr('id');

        if (active_user_name == original_user) {

            $('#message_to').val(new_username);
        }
        for (var i = 0; i < active_users_count; i++) {

            if (jQuery.trim($('.active_us').eq(i).text()) == original_user) {

                //change user_name
                $('.active_us').eq(i).find('p').text(new_username);

                //if tab with the name is already present change that tab name and div id too

                if ($('#' + original_user).length != "0") {
                    //change div id
                    $('#' + original_user).attr('id', new_username);

                    //change tab id
                    $('#' + original_user + '_User').attr('id', new_username + '_User');
                    $('#' + new_username + '_User').attr('href', new_username);
                    $('#' + new_username + '_User').attr('this_val', new_username);
                    $('#' + new_username + '_User').text(new_username);

                    //change id of div that contains message
                    $('#discussion_' + original_user).attr('id', 'discussion_' + new_username);
                }

            }
        }
        //change message_to to new username since name is changed

        //add change details in notification
        var old_count = $('.count').eq(0).text();

        if (old_count == "") {
            old_count = 0;
        }
        var notif_count = parseInt(old_count) + 1;
        $('.count').eq(0).text(notif_count);
        //add count in notification count
        $('#notiContent').prepend('<li><strong>' + original_user + '</strong> has changed his/her name to <strong>' + new_username + '</strong></li>')
    }

    //get sent images
    chat.client.receive_files = function (message_from, message_to, file_path) {

        if ((message_to == $('#displayname').val() || message_to == "All") && message_from != $('#displayname').val()) {
            //add image in the list of received images

            $('#images').append('<div class="image-show col-sm-8" style="float:left;" this_link="/shared images/' + file_path + '"><strong>' + message_from + ':</strong><img class="img-responsive" style="width:50px;height:50px;" src="/shared images/' + file_path + '"></div>')
            $('.image-show').on('click', function () {
                //show image in next window
                var win = window.open($(this).attr('this_link'), "Image");
                if (win) {
                    win.focus();
                }
                else {
                    alert('Please allow popups for this website');
                }
            });
        }
    }

    //get username and store it to prepend to messages.
    $('#displayname').val(prompt('Enter your name:', ''));
    if ($('#displayname').val() == "" || $('#displayname').val() == null) {
        location.reload(true);
    }

    //set initial focus to message input box
    $('#message').focus();
    //start the connection.
    $.connection.hub.start().done(function () {
        if ($('#displayname').val() == "" || $('#displayname').val() == null) {
            location.reload(true);
        }
        chat.server.check_username($('#displayname').val());
        saveUsernameOnConnection();
        //load active users from database
        getActiveUsers();

        //change username function
        $('#change_username').on("keypress", function () {
            var new_user_name = $('#change_username').val();
            var keycode = (event.keyCode ? event.keyCode : event.which);
            if (keycode == '13') {
                if (new_user_name == "" || new_user_name == null) {
                    alert("Username cannot be empty.");
                    return;
                }
                chat.server.changeUsername($('#displayname').val(), new_user_name);
                $('#displayname').val(new_user_name);
                alert("Username change successful.");
                $('#message').focus();
                $('#change_username').val('');
            };
        });
    });

    //store value in message_to field
    $(document).on('click', '.a-white', function () {
        var user_name = $(this).attr('this_val');

        $('#message_to').val(user_name);
        //change the name(1) to name as the message will be seen
        $('#' + user_name + '_User').text(user_name);
    });

    $('.noti').click(function (e) {
        e.stopPropagation();
        $('.noti-content').hide();
        var noti_count = $('.count').eq(0).text();
        if (noti_count != "") {
            $('.noti-content').show();
            $('.count').eq(0).text('');
        };
    });

    $('#send_file').on('click', function () {
        uploadimage();
    })

    //page closing event
    //remove username from database
    $(window).on("beforeunload", function () {

        chat.server.removeUsername($('#displayname').val());
    });

    $('#message').on("keypress", function () {
        var keycode = (event.keyCode ? event.keyCode : event.which);
        if (keycode != '13') {
            return;
        }
        if ($(this).val() == "") {
            return;
        }
        var countt = $('.active_us').length;
        var user_count = 0;
        for (var i = 0; i < countt; i++) {
            if (jQuery.trim($('.active_us').eq(i).text()) == $('#message_to').val()) {
                user_count++;
            }
        }
        if (user_count == 0) {
            alert("User is offline.");
            return;
        }
        if ($('#message_to').val() == "" || $('#message_to').val() == null) {
            alert("Please choose a receiver from RHS");
            return;
        }
        //call the send method on the hub
        chat.server.send($('#displayname').val(), $('#message').val(), $('#displayname').val(), $('#message_to').val());

        //clear text box and reset focus for next comment.
        $('#message').val('').focus();
    });

    $(document).on('click', '.active_us', function () {
        var txt = $(this).find('p').text();
        $('#message_to').val(txt);
        $('#message').focus();

        let isTabUnavailableOfUser = $(`#${txt}`).length == 0;
        if (isTabUnavailableOfUser) {
            addTabs(txt);
           
        }
        $(`#${txt}_User`).click();
    });
});

function appendMessage(userName, encodedName, encodedMessage, floatDirection, textAlign = 'left') {
    //add message
    $('#discussion_' + userName).prepend(`<div class="col-sm-8" style="float:${floatDirection};text-align:${textAlign};font-size:18px;"><strong>${encodedName}</strong>:&nbsp;&nbsp;${encodedMessage}</div>`);
}

function addTabs(userName) {
    $('.nav-tabs').append('<li><a data-toggle="tab" href="#' + userName + '" class="a-white" id="' + userName + '_User" this_val=' + userName + '>' + userName + '(1)</a></li>');

    //add new div-content and prepend a message
    $('.tab-content').prepend('<div id="' + userName + '" class="tab-pane fade"><div class="container-fluid" id="discussion_' + userName + '"></div></div>');
}

function uploadimage() {
    var file = $('#imagebrowse').get(0).files;
    var data = new FormData;
    data.append("ImageFile", file[0]);
    data.append("message_from", $('#displayname').val());
    data.append("message_to", $('#message_to').val());
    $.ajax({
        type: "POST",
        url: "/test/send_files",
        data: data,
        contentType: false,
        processData: false,
        success: function (content) {

            if (content.success == false) {
                alert(content.responseText);
            }
            else {
                //add image in the image div class
                $('#images').append('<div class="image-show col-sm-8" style="float:right;" this_link="/shared images/' + content.file_path + '"><img class="img-responsive" style="width:50px;height:50px;float:right;" src="/shared images/' + content.file_path + '"></div>')
                $('.image-show').on('click', function () {
                    //show image in next window
                    var win = window.open($(this).attr('this_link'), "Image");
                    if (win) {
                        win.focus();
                    }
                    else {
                        //Browser has blocked it
                        alert('Please allow popups for this website');
                    }
                });
            }
        },
        error: function (response) {
            alert(response.responseText);
        }
    });
}

function saveUsernameOnConnection() {
    var user_name = $('#displayname').val();
    chat.server.saveUserDetails(user_name);
    //send username to server so that it can be used to broadcast to all clients
    chat.server.shareUsername(user_name);
}

function getActiveUsers() {
    var name = $('#displayname').val();
    // alert(name);
    $.ajax({
        url: "/test/index",
        type: "POST",
        data: JSON.stringify({ user_name: name }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (users) {
            $.each(users, function (i, res) {
                $('#active_users').append("<div class='form-group active_us'>                 <p class='form-control text-primary' >" + res["user_name"] + "</p></div>");
            });
        },
        error: function (data) {
            alert(data);
        }
    });
};