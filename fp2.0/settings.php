<?php
    require('helper.php'); // checks https and session status
?>
<html>
    <head>
        <?php output_dependencies() ?>
        <style>
            .form-control
            {
                max-width: 250px;
            }
        </style>
    </head>
    <body>
        <div class="content">
            <!-- Navigation Bar -->
            <div class="row">
                <div class="col-sm-12">
                    <nav class="navbar navbar-default">
                        <div class="container-fluid">
                            <!-- Title of the Application -->
                            <div class="navbar-left">
                                <h2>Günter Hans Reporting Application</h2>
                            </div>

                            <!-- Logout Button -->
                            <div class="navbar-right">
                                <div class="centerdiv">
                                    <a class="navbar-brand" href="/fp2.0/">
                                        <h4>Home</h4>
                                    </a>
                                    <a class="navbar-brand" href="logout.php">
                                        <h4>Logout?</h4>
                                    </a>
                                </div>
                            </div>
                        </div>
                    </nav>
                </div>
            </div>
                <label for="inputCurrentPass" class="sr-only">Current Password</label>
                <input type="password" name="currentPass" id="inputCurrentPass" class="form-control" size="10" placeholder="Current Password" required autofocus>
                <br>
                <label for="inputNewPass" class="sr-only">New Password</label>
                <input type="password" name="newPass" id="inputNewPass" class="form-control" size="10" placeholder="New Password" required>
                <br>
                <label for="inputNewPass" class="sr-only">New Password</label>
                <input type="password" name="verify" id="inputVerify" class="form-control" size="10" placeholder="Verify New Password" required>
                <br>
                <input id="changePassSubmit" class="btn btn-lg btn-primary btn-clock" type="submit" disabled="true" value="Change Password">
            <div id="verifyMessage">
            </div>
        </div>
    <script>
        /* function that sends user input to verify.php, based on what is sent, the password will be updated,
        or the user will be notified if inputs need to be adjusted or if there was a database error */
        function sendData(changePassSubmit)
        {
            var currentPass = $('#inputCurrentPass').val();
            var newPass = $('#inputNewPass').val();
            var verifyPass = $('#inputVerify').val();
            var input = 
            {
                currentPass: currentPass, 
                newPass: newPass, 
                verifyPass: verifyPass, 
                changePassSubmit: changePassSubmit
            };
            if (currentPass && newPass && verifyPass)
            {
                $.post('verify.php', input, function(data)
                {
                    $('#verifyMessage').html(data);
                    if (data == "Passwords don't match.")
                        $('#changePassSubmit').prop('disabled', true);
                    else
                        $('#changePassSubmit').prop('disabled', false);
                });
            }
            else if (changePassSubmit)
                $('#verifyMessage').html('All fields are required, please fill them in.');
        }
        $(document).ready(function()
        {
            $('#inputVerify').keyup(function()
            {
               sendData(false);
            });
            $('#inputNewPass').keyup(function()
            {
                sendData(false);
            });
            $('#changePassSubmit').click(function()
            {
                sendData(true);
            });
            /*
            $('input.form-control').keyup(function(event){
                if (event.keyCode == 13) // enter key
                    $('#changePassSubmit').click(); 
                // this if will only 'click' if the button is enabled
            });
            /* this code allows for submission by hitting enter but it is removed for now
            because with the current logic, once could submit by hitting enter regardless of
            matched new password and verify password, causing unwanted password change.
            to fix this issue change the new and verify password matching to strictly javascript in this
            file, don't bother sending to verify.php unless those passwords are matched*/
        });
    </script>
    </body>
</html>
