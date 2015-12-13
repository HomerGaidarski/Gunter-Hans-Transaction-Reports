<?php
/* this file handles logouts */

	// force https
	if ($_SERVER['HTTPS'] != 'on')
	{
   		header("location: " . 'https://' . $_SERVER['HTTP_HOST'] . $_SERVER['REQUEST_URI']);
    	exit;
	}

	session_start();

	session_unset();



	// destroy the session
	session_destroy();

	header("Location: login.php");
	exit;
?>
