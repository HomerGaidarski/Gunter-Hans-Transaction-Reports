<?php

require_once('helper.php');

$rType = get_required_post_var('rType');
$sDate = get_required_post_var('sDate');
$eDate = get_required_post_var('eDate');
$days = get_required_post_var('days');
$sTime = get_required_post_var('sTime');
$eTime = get_required_post_var('eTime');

switch($rType)
{
    case "totalSales":
   		generate_total_sales();
		exit;
    case "discountsGiven":
        generate_discounts_given();
        exit;
    case "numberOfTransactions":
        generate_number_of_transactions();
        exit;
    case "salesPerTransaction":
		generate_sales_per_transaction();
        exit;
    case "tipsPerSaleByEmployee":
        generate_tips_per_sale();
        exit;
    case "aveTipPercentByEmployee":
        generate_average_tip_percent_by_employee();
        exit;
    default:
        exit;
}

function generate_total_sales()
{
	$beginningQuery = "SELECT tran_date, tran_time, sum(total) as dailyTotal FROM gh_transaction";
	$endQuery = "GROUP BY tran_date";
	generate_common('line', $beginningQuery, $endQuery, 'tran_date', NULL, 'dailyTotal');
}

function generate_discounts_given()
{
	$beginningQuery = "SELECT gh_employee.emp_id, fname, lname, sum(discount) as empDiscount FROM gh_transaction INNER JOIN gh_employee ON (gh_transaction.emp_id = gh_employee.emp_id)";
	$endQuery = "GROUP BY gh_employee.emp_id ORDER BY lname";
	generate_common('bar', $beginningQuery, $endQuery, 'fname', 'lname', 'empDiscount');
}

function generate_number_of_transactions()
{
	$beginningQuery = "SELECT tran_date, tran_time, COUNT(id) as numOfTransactions FROM gh_transaction";
	$endQuery = "GROUP BY tran_date";
	generate_common('line', $beginningQuery, $endQuery, 'tran_date', NULL, 'numOfTransactions');
}

/*
	Assigned to Homer
*/
function generate_sales_per_transaction()
{
	$beginningQuery = "SELECT tran_date, tran_time, avg(total) as averageTotal FROM gh_transaction "; // initial query with 0 restrictions; gets the average of the transactions for each day
	$endQuery = "GROUP BY tran_date";
	generate_common('line', $beginningQuery, $endQuery, 'tran_date', NULL, 'averageTotal');
}

/*
    Assigned to Kean
*/
function generate_tips_per_sale()
{
    $beginningQuery = "SELECT tran_date, tran_time,  avg(tip_amount) as averageDailyTip FROM gh_transaction ";
	$endQuery = "GROUP BY tran_date";
	generate_common('line', $beginningQuery, $endQuery, 'tran_date', NULL, 'averageDailyTip');
}

/*
    Assigned to Mike
*/
function generate_average_tip_percent_by_employee()
{
    $beginningQuery = "SELECT gh_transaction.emp_id, fname, lname, ROUND((avg(tip_amount/subtotal) * 100), 2) as averageTipPercent FROM gh_transaction INNER JOIN gh_employee ON gh_transaction.emp_id = gh_employee.emp_id ";
	$endQuery = "GROUP BY emp_id ORDER BY lname";
	generate_common('bar', $beginningQuery, $endQuery, 'fname', 'lname', 'averageTipPercent');
}

//Creates the conditional necessary to select the 
function generate_conditional()
{
	global $sDate, $eDate, $days, $sTime, $eTime;
    /* will return a string. to be placed in a SQL statment should be able to do: SELECT * where(generateConditional); 
    sdate edate and time of day are going to be strings
    time of day is going to be "00:00:00 - 00:00:00"  AND DATES ARE "YYYY-MM-DD"
    The mysql functions MONTH(DATE) AND DAYOFWEEK(DATE) work with that format
    array of days will be an array of integers, those intergs represent the days of the week 1 = sunday 7 = saturday*/

    $returnString = '';
    
	$sDate = formatDate($sDate);
	$eDate = formatDate($eDate);

    //Calculate $time1 and $time2
    $sTime = trim($sTime) . ":00"; // array of strings 0 contains "00:00:00 " so trim the spaces
    $eTime = trim($eTime) . ":59";
    
    // next we need to loop through array of days and add each day of the week to the return string sql statment
    $returnString = 'tran_date >= ? AND tran_date <= ? AND tran_time >= ? AND tran_time <= ?'; 

    //Implement DAYOFWEEK Conditional
    $returnString .= ' AND (';
 	 
    foreach ($days as $day)
    {
        if (!is_null($day))
        {
			if (is_int((int) $day)) // make sure this is in fact an integer and not attempted sql attack
			{
				if ($day >= 1 && $day <= 7) // valid integer
            		$returnString .= "DAYOFWEEK(tran_date) = $day OR ";
        	}
		}
    }
  
    //Remove last OR with spacing
  	$returnString = substr($returnString, 0, strlen($returnString) - 4);
    
    //Close final parenthesis
    $returnString .= ')';
       
    return $returnString;
}

// reformats a MM/DD/YYYY date to YYYY-MM-DD date
/* error checking for this function is important for sql injection prevention since monthlyReport.php uses
get variables for start and end date */
function formatDate($date)
{
	$date = trim($date, " "); // removes whitespace
	$pieces = explode("/", $date); // $pieces[0] is month, $pieces[1] is day, $pieces[2] is year
	if (sizeof($pieces) != 3) // date not formatted correctly or is not a date, quit
	{
		$pieces = explode("-", $date);
		if (sizeof($pieces) != 3)
			exit;
	}
	$month = $pieces[0];
	$day = $pieces[1];
	$year = $pieces[2];

	// is_int does not return true if the integer is a string type, so cast it to int first
	if (is_int((int)$month) && is_int((int)$day) && is_int((int)$year))
	{
		if (!(($month <= 12 || $month >= 1) && ($day >= 1 || $day <= 31) && $year > 1990))
		{
			exit; // if any part of date isn't formatted right exit immediately
		}
	}
	else // if one or more variables are not integers exit immediately
		exit;
	
	return $year . '-' . $month . '-' . $day; // return reformatted date YYYY-MM-DD
}

// Homer
function stmt_bind_assoc(&$stmt, &$out) // dynamic way for binding all return variables
{
	$data = mysqli_stmt_result_metadata($stmt);
	$fields = array();
	$out = array();
	$fields[0] = $stmt;
	for ($i = 1; $field = mysqli_fetch_field($data); $i++)
	{
		$fields[$i] = &$out[$field->name];
	}
	call_user_func_array('mysqli_stmt_bind_result', $fields);
}

//Homer
function generate_common($graphType, $beginningQuery, $endQuery, $label1Column, $label2Column, $valueColumn)
{
	global $rType, $sDate, $eDate, $days, $sTime, $eTime;
	$link = retrieve_mysqli();
	$query = $beginningQuery . " WHERE ";
	$query .= generate_conditional() . $endQuery;
	if ($stmt = mysqli_prepare($link, $query))
	{
		mysqli_stmt_bind_param($stmt, "ssss", $sDate, $eDate, $sTime, $eTime);
		mysqli_stmt_execute($stmt);
		$stmt->store_result();
		$resultrow = array();
		stmt_bind_assoc($stmt, $resultrow);
		$numRows = mysqli_stmt_num_rows($stmt);


		if ($numRows != 0)
		{
			$isEmployee = FALSE;
			if (isset($label2Column))
				$isEmployee = TRUE;
			$labels = array();
			$values = array();
			while (mysqli_stmt_fetch($stmt))
			{
				$label1 = NULL;
				$label2 = NULL; // label2 is lastname if isEmployee, otherwise NULL
				$value = NULL;
				foreach($resultrow as $key => $data)
				{
					if ($label1Column == $key)
						$label1 = $data;
					else if ($valueColumn == $key)
						$value = $data;
					else if ($isEmployee)
					{
						if ($label2Column == $key)
							$label2 = $data;
					}
					else if (isset($label1) && isset($value))
						break;
					if (isset($label1) && isset($value) && isset($label2))
						break;
				}
				$label = $label1;
				if ($isEmployee)
				{
					$label .= ' ' . $label2;
				}
				
				array_push($labels, $label);
				array_push($values, $value);
			}
			mysqli_stmt_close($stmt);
			echo json_encode(array(
				'graphType' => $graphType,
				'labels' => $labels,
				'values' => $values
			));
			exit;
		}
		echo '0 results returned.';
		exit;
	}
}

//mysqli_close($mysqli);

?>
