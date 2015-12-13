<?php
	require('helper.php'); // checks https and session status
?>
<html>
    <head>
        <?php output_dependencies() ?>
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
                                    <a class="navbar-brand" href="logout.php">
                                        <h4>Logout?</h4>
                                    </a>
                                </div>
                            </div>
                        </div>
                    </nav>
                </div>
            </div>

            <!-- Control Bar -->
            <div class="row">
                <div class="col-sm-3">
                    <div class="panel panel-primary">
                        <div class="panel-heading">
                            <h3>Report Selection</h3>
                        </div>
                        <div class="panel-body">
                            <table class="table">
                                <tr>
                                    <td>
                                        <h4>Report Type:</h4>
                                        <select id="reportType" onchange="retrieveReportAndPopulateCradle()" class="selectpicker" data-width="100%">
                                            <option value="totalSales">Total Sales</option>
                                            <option value="discountsGiven">Discounts Given</option>
                                            <option value="numberOfTransactions">Transaction Count</option>
                                            <option value="salesPerTransaction">Sales Per Transaction</option>
                                            <option value="tipsPerSaleByEmployee">Tips Per Sale</option>
                                            <option value="aveTipPercentByEmployee">Average Tip Percent</option>
                                        </select>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <h4>Date Range:</h4>
                                        <div class="col-sm-6">
                                            <input id="dateRange" type="text" class="form-control daterangepicker" value="01/01/2012 - 11/24/2015" />
                                        </div>
                                        <br />
                                        <br />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <h4>Start Time:</h4>
                                        <div class="input-group clockpicker" data-placement="right" data-autoclose="true">
                                            <input id="startTime" type="time" class="form-control" value="00:00">
                                            <span class="input-group-addon">
                                                <span class="glyphicon glyphicon-time"></span>
                                            </span>
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <h4>End Time:</h4>
                                        <div class="input-group clockpicker" data-placement="right" data-autoclose="true">
                                            <input id="endTime" type="time" data-placement="right" class="form-control" value="23:59">
                                            <span class="input-group-addon">
                                                <span class="glyphicon glyphicon-time"></span>
                                            </span>
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <h4>Transaction Day:</h4>
                                        <div><input type="checkbox" id="1" checked>Sun</input></div>
                                        <div><input type="checkbox" id="2" checked>Mon</input></div>
                                        <div><input type="checkbox" id="3" checked>Tue</input></div>
                                        <div><input type="checkbox" id="4" checked>Wed</input></div>
                                        <div><input type="checkbox" id="5" checked>Thu</input></div>
                                        <div><input type="checkbox" id="6" checked>Fri</input></div>
                                        <div><input type="checkbox" id="7" checked>Sat</input></div>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <br />
                                        <button id="refresh" type="button" class="button form-control btn-primary">
                                            <span class="glyphicon glyphicon-refresh" aria-hidden="true"></span> Generate Graph
                                        </button>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </div>
                </div>
                
                <div class="col-sm-9">
                    <div class="panel panel-info">
                        <div class="panel-heading">
                            <h3 id="header">Report Header</h3>
                        </div>
                        
                        <div class="panel-body" id="constraints">
                            <div id="cradle">
                                <canvas id="chart"></canvas>
                            </div>
                            <div id="progressDisplayBackground">
                                <span id="loaderIcon" class="glyphicon glyphicon-refresh " aria-hidden="true"></span>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-sm-11">

                </div>
                <div class="col-sm-1">
                </div>
            </div>
            <div class="row">
                <div class="col-sm-12">
                    <!-- This is where the graphs will go -->
                </div>
            </div>

            
        </div>
            
        <?php output_plugin_activations(); ?>
        <script>
            var chart;
            var context = $("#chart").get(0).getContext("2d");
           	
            function hideLoader()
            {
                $("#progressDisplayBackground").css({"opacity": "0", "pointer-events": "none"});
            }
            
            function showLoader()
            {
                $("#progressDisplayBackground").css({"opacity": ".4", "pointer-events": "none"});
            }
            
            function getDays()
            {
                var days = [];
                
                for (i = 1; i <= 7; ++i)
                {
                    var isChecked = $("#" + i.toString()).get(0).checked;
                    
                    if (isChecked)
                    {
                        days.push(i);
                    }
                }
         
				if (days === undefined || days.length == 0)
				{
					alert('At least one day of the week must be checked.');
					hideLoader();
					return; // returns undefined (nothing)
				}
       
                return days;
            }
            
            function drawGraph(data)
            {
                //Clear last Graph if necessary
                if (chart != undefined)
                {
                    chart.destroy();
                }
                
                if (data.graphType == 'line')
                {                    
                    //Custom JS
                    // Get context with jQuery - using jQuery's .get() method.
                    
                    // This will get the first returned node in the jQuery collection.
                    var data = {
                        labels: data.labels,
                        datasets: [
                            {
                                label: "Report",
                                fillColor: "rgba(200,235,255,0.5)",
                                strokeColor: "rgba(220,220,220,1)",
                                pointColor: "rgba(220,220,220,1)",
                                pointStrokeColor: "#fff",
                                pointHighlightFill: "#fff",
                                pointHighlightStroke: "rgba(220,220,220,1)",
                                data: data.values
                            }
                        ]
                    };
                    
                    chart = new Chart(context).Line(data, {
                        scaleShowVerticalLines: false,
                        responsive: true,
                        showTooltips: false
                    });
                }
                else if (data.graphType = 'bar')
                {
                    var data = {
                        labels: data.labels,
                        datasets: [
                            {
                                label: "Report",
                                fillColor: "rgba(200,235,255,0.5)",
                                strokeColor: "rgba(220,220,220,1)",
                                pointColor: "rgba(220,220,220,1)",
                                pointStrokeColor: "#fff",
                                pointHighlightFill: "#fff",
                                pointHighlightStroke: "rgba(220,220,220,1)",
                                data: data.values
                            }
                        ]
                    };
                    
                    chart = new Chart(context).Bar(data, {
                        response: true,
                    });
                }
                
            }
            
            function retrieveReportAndPopulateCradle()
            {
                showLoader();
                
                //url to send post request to
                var url = "graphs.php";
                
                //get report type
                var reportTypeElement = document.getElementById("reportType");
                var reportType = reportTypeElement.value;
                document.getElementById("header").innerHTML = reportTypeElement.options[reportTypeElement.selectedIndex].text;
                
                //gather data
                var startTime = $("#startTime").val();
                var endTime = $("#endTime").val();
                var dateRange = $("#dateRange").val();
                var split = dateRange.split("-");
                var startDate = split[0];
                var endDate = split[1];
                var daysArray = getDays();
				if (daysArray === undefined) // prevents user from having no days of week selected
					return;
				// need to add error checking for other inputs as well
		                
                //excute post request
                $.post(url, {rType: reportType, sDate: startDate, eDate: endDate, days: daysArray, sTime: startTime, eTime: endTime}, function(data) {
					console.log(data);
					data = $.parseJSON(data);
					drawGraph(data);
					hideLoader();
				 });
            }
            
            $(document).ready
            ( function() {
                
                retrieveReportAndPopulateCradle();
                
                $("#refresh").click
                (function ()
                 {
                    retrieveReportAndPopulateCradle();
                });
            });
        </script>
    </body>
</html>
