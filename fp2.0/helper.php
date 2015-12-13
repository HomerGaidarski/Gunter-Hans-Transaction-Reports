<?php

require_once('session.php');

//ini_set('display_errors', 1);

function get_required_post_var($name)
{
    if (isset($_POST[$name]))
    {
        return $_POST[$name];
    }
    
    //no variable exists with that name so cancel the script
    exit;
}

function output_dependencies()
{
    echo('
        <title>Günter Hans Reports</title>
        <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
        <link rel="stylesheet" href="css/bootstrap.min.css">
        <link rel="stylesheet" type="text/css" href="css/bootstrap-select.min.css">
        <link rel="stylesheet" type="text/css" href="css/daterangepicker.css">
        <link rel="stylesheet" type="text/css" href="css/bootstrap-timepicker.min.css">
        <link rel="stylesheet" type="text/css" href="css/bootstrap-clockpicker.min.css">
        <link rel="stylesheet" type="text/css" href="css/cradle.css"> <!-- css for index.php -->
        <link rel="icon" href="images/GunterHansLogo.jpg" type="image/gif"> <!-- Günter Hans tab icon -->
        <script src="js/jquery-1.11.3.min.js"></script>
        <script src="js/bootstrap.min.js"></script>
        <script src="js/bootstrap-select.min.js"></script>
        <script src="js/moment.min.js"></script>
        <script src="js/daterangepicker.js"></script>
        <script src="js/bootstrap-timepicker.min.js"></script>
        <script src="js/bootstrap-clockpicker.min.js"></script>
        <script src="js/Chart.min.js"></script>
    ');
}

function output_plugin_activations()
{
    echo ('<script type="text/javascript">
        $(".selectpicker").selectpicker(); 
        $(".daterangepicker").daterangepicker(
        {
            ranges: 
            {
                "Today": [moment(), moment()],
                "Yesterday": [moment().subtract(1, "days"), moment().subtract(1, "days")],
                "This Week": [moment().startOf("weeks"), moment().endOf("weeks")],
                "Year to Date": [moment().startOf("years"), moment()],
                "Last Year": [moment().subtract(1, "year").startOf("years"), moment().subtract(1, "year").endOf("years")],
                "All Time": [moment().subtract(3, "year").startOf("years"), moment()]
            }
         }); 
        $(".timepicker").timepicker(); 
        $(".clockpicker").clockpicker({donetext: "Done"});
    </script>');
}   

function retrieve_mysqli()
{
    return mysqli_connect("", "", "", ""); //or die ("Connection Error " . mysqli_error($link));
}

?>
