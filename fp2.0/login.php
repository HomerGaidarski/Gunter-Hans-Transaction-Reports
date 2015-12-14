<?php require_once('helper.php'); ?>

<!DOCTYPE html>
<html>
<head>
	<?php output_dependencies() ?>
	<style>
		body
		{
			padding-top: 40px;
			padding-bottom: 40px;
			background-color: #eee;
		}
        .text-centered
        {
            text-align: center;
        }
		.form-signin 
		{
	 		max-width: 330px;
	    	padding: 15px;
			margin: 0 auto;
		}
		.form-signin .form-signin-heading, .form-signin .checkbox 
		{
			margin-bottom: 10px;
		}
		.form-signin .checkbox 
		{
	  		font-weight: normal;
		}
		.form-signin .form-control 
		{
			position: relative;
			height: auto;
			-webkit-box-sizing: border-box;
			-moz-box-sizing: border-box;
			box-sizing: border-box;
			padding: 10px;
			font-size: 16px;
            margin-bottom: 4px;
		}
		.form-signin .form-control:focus
		{
	  		z-index: 2;
		}
		.form-signin input[type="email"]
		{
	  		margin-bottom: 4px;
		}
		.form-signin input[type="password"]
		{
	  		margin-bottom: 10px;
		}
		p
		{
			text-align: center;
		}
	</style>
</head>
<body>	
    <h1 class="text-centered">Please Login...</h1>
	<div class="container">
		<form class="form-signin" action="login.php" method="post">
			<label for="inputUser" class="sr-only">Username</label>
			<input type="text" name="username" id="inputUser" class="form-control" size="10" placeholder="Username" required autofocus>
			<label for="inputPassword" class="sr-only">Password</label>
			<input type="password" name="password" id="inputPassword" class="form-control" size="10" placeholder="Password" required>
			<button class="btn btn-lg btn-primary btn-clock" type="submit">Login</button>
		</form>
<?php

	//If the user tried to login....
	if (isset($_POST['username']) && isset($_POST['password']))
	{
		$username = htmlspecialchars($_POST['username']);
		$password = htmlspecialchars($_POST['password']);

        if (checkCredentials($username, $password))
        {
            $_SESSION['loggedin'] = $username;
            if (isset($_SESSION['monthlyReportURL']))
            {
            	redirect(basename($_SESSION['monthlyReportURL']));
           	}
           	else
           	{
           		redirect("index.php");
           	}
        }
        else
        {
            echo '<p>Username or password is invalid.</p>';
        }
	}
?>
	</div>
</body>
</html>