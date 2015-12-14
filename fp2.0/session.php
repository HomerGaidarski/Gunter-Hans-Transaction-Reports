<?php
	session_start();

	session_regenerate_id();

	function isLoggedIn()
	{
		if (isset($_SESSION["loggedin"]))
			return true;
		return false;
	}

	/* this file is for checking session variables and redirecting users depending on whether they
	are logged in or not */
	


	// force https
	if ($_SERVER['HTTPS'] != 'on')
	{
		header("location: " . 'https://' . $_SERVER['HTTP_HOST'] . $_SERVER['REQUEST_URI']);
		exit;	
	}
	if (false !== strpos($_SERVER['REQUEST_URI'], 'monthlyReport.php'))
	{
		$_SESSION['monthlyReportURL'] = $_SERVER['REQUEST_URI'];
	}

	// if user is not logged in, redirect to login page
	if (!isLoggedIn())
	{
		redirect("login.php"); // only redirect if not already at the login page
	}
	else// if user is logged in, redirect them to home page 
	{
		redirect("index.php");
	}
	/* this function prevents infinite redirect loops */
	function redirect($page)
	{
		$currentPage = basename($_SERVER['PHP_SELF']);
		// prevent redirecting away from graphs.php when $.post is accessing
		if ($currentPage == 'graphs.php' || $currentPage == 'verify.php')
		{
			if ($_SERVER['HTTP_X_REQUESTED_WITH'] != 'XMLHttpRequest')
			{
				header('Location: /fp2.0/');
				exit;
			}
		}
		else if ($currentPage != $page)
		{
			if ($page == 'index.php')
				$page = ''; // make sure index.php is not included so that the url is shorter
			
			if (!isLoggedIn())
			{
				header("Location: login.php");
				exit;
			}
			else if ($currentPage != 'monthlyReport.php' && $currentPage != 'settings.php')
			{
				header("Location: /fp2.0/" . $page);
				exit;
			}
		}
	}
?>