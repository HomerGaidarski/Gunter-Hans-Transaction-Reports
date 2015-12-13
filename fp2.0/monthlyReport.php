<?php
	require_once('helper.php'); // checks https and session status
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

            <!-- Control Bar -->
            <div class="row">
                <div class="col-sm-9">
                    <div class="panel panel-info">
                        <div class="panel-heading">
                            <h3 id="header"></h3>
                        </div>
                        <form method="GET">
                            <input name="startDate" id="startDate" type="hidden">
                            <input name="endDate" id="endDate" type="hidden">
                        </form>
                        
                        <div class="panel-body" id="constraints">
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
                            </table>
                            <div id="cradle">
                                <canvas id="chart"></canvas>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <?php output_plugin_activations(); ?>
        <script>
            var chart;
           	
            function drawGraph(data)
            {
                var context = $("#chart").get(0).getContext("2d");
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
            
            var monthNames = ["January", "February", "March", "April", "May", "June",
  "July", "August", "September", "October", "November", "December"];
            var month;

            function retrieveReportAndPopulateCradle()
            {
                //url to send post request to
                var url = "graphs.php";

                // need to add 1 to the month because getMonth() returns 0-11
                // + 1 is commented out below because no data is available for decemeber
                var $_GET = <?php echo json_encode($_GET); ?>;
                var startDate = $_GET['startDate'];
                var endDate = $_GET['endDate'];
                console.log(startDate + " - " + endDate);

                month = monthNames[startDate.split('-')[0] - 1];

                var daysArray = [1,2,3,4,5,6,7];
                    //excute post request

                    var reportType = $("#reportType").val();
                     document.getElementById("header").innerHTML = month + " Report";
                    // m - 1 only because no data exists for december (as I type this, it is december 2015)

                    $.post(url, {rType: reportType, sDate: startDate, eDate: endDate, days: daysArray, sTime: '00:00', eTime: '23:59'}, function(data) {
					   console.log(data);
					   data = $.parseJSON(data);
                        drawGraph(data);
				    });
            }
            
            $(document).ready
            ( function() {
                retrieveReportAndPopulateCradle();
            });
        </script> 
    </body>
</html>