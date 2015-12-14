<?php
	require_once('helper.php');

	/* when input fields that are empty are sent using jquery, the values are technically set,
	this function will catch those empty fields */
	function myIsset($variable)
	{
		if (!isset($variable) || trim($variable) == '')
			return false;
		else
			return true;
	}

	if (myIsset($_POST['newPass']) && myIsset($_POST['verifyPass']) && $_POST['changePassSubmit'] != 'true')
	{
		if ($_POST['newPass'] == $_POST['verifyPass'])
			echo '';
		else
			echo "Passwords don't match.";
	}
	else if (myIsset($_POST['currentPass']) && myIsset($_POST['newPass']) && $_POST['changePassSubmit'] == 'true')
    {
        $pass = htmlspecialchars($_POST['currentPass']);
        $user = $_SESSION['loggedin'];
     	if (checkCredentials($user, $pass))
        {
            $newPass = htmlspecialchars($_POST['newPass']);
            if ($newPass == $pass) // if the passwords are the same don't even bother updating the database
            {
                echo 'Your new password must be different from your current password.';
                exit;
            }


            $link = retrieve_mysqli();
            $queryString = 'UPDATE user SET salt = ?, hashed_password = ? WHERE username = ?';

            // get salt and hash password
            mt_srand();
            $salt = mt_rand();
            $hashPass = generateHash($salt . $newPass);

            // query the database
            if ($stmt = mysqli_prepare($link, $queryString))
            {
                //Get the stored salt and hash as $dbSalt and $dbHash
                mysqli_stmt_bind_param($stmt, "sss", $salt, $hashPass, $user);
                mysqli_stmt_execute($stmt);
                mysqli_stmt_close($stmt); // close prepared statement
                mysqli_close($link);
                echo 'Password succesfully changed.';
            }
            else
            	echo 'Error, password was not changed.';
        }
       	else
        	echo 'Current password is incorrect, nothing was changed.';
    }
?>