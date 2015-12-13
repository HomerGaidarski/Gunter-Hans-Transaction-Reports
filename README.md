# Günter Hans Transaction Reports
Transaction statistics website for the European Pub &amp; Cafe, <a href="http://www.gunterhans.com/">Günter Hans</a>, located in Columbia, MO.

The owner of Günter Hans had transaction data all in csv files and wanted a way to view it in a more visual and interactive way. A team of 4 students: Jared Melton, Homer Gaidarski, Mike Witte, Kean Mattingly, implemented a database and built a website to solve this problem.

The site is very simple, it really only has one significant page that the user visits, index.php, which can only be accessed after login. There is no register option on the site because the site is primarily for one user and for this reason I did not include the url of the website anywhere in this repository. At index.php, the user can generate graphical transaction reports that utilize by altering various selection boxes. There is also a monthly reporting feature that emails a generated url to a specific email from a python script that runs on the server every month via crontab. The url will take the user to a page that only shows data for a single month. Boostrap is primarily used for the CSS and chart.js is used to make the graphs.

There are 6 types of reports, for line graphs, the x-axis is always dates, for bar graphs, the x-axis is always employee names:

1. Total Sales - shows a line graph with the total amount of money earned on the y-axis

2. Discounts Given - shows a bar graph with the total discount cost on the y-axis

3. Transaction Count - shows a line graph with the total number of transactions on the y-axis

4. Sales Per Transaction - shows a line graph with the average amount of money made in a transaction on the y-axis

5. Tips Per Sale - shows a line graph with the average tip amount on the y-axis

6. Average Tip Percent - shows a bar graph with the average tip percent on the y-axis

For all these reports, the user can set the start date, end date, start time, end time, and can even select which days of the week to include. The date range pulls up a calendar when clicked and also includes quick selection features such as selecting Today, Yesterday, This Week, etc. The start and end time selectors have a clock button that allows the user to set the time from an analog clock perspective. So all these selections just need a mouse to operate, but if the user wants to edit with the keyboard they can do that of course too. Being able to change these time parameters allows the user to see how business is doing during certain times and days and take note of when they will be busy, and when they won't be busy so they can plan accordingly by getting more or less employees to work for those times and days.
