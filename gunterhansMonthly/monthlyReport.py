import smtplib, time, datetime, calendar

from email.mime.multipart import MIMEMultipart
from email.mime.text import MIMEText

class gunterhansMonthly:

	def sendEmail(self):

		# SMTP server host information
		mail = smtplib.SMTP('smtp.sendgrid.net', 587)
		mail.ehlo()
		mail.starttls()

		#username and password of smtp server
		mail.login('', '')
		
		# get first and last day of the previous month and generate the url
		# str() casts passed in parameter to a string
		now = datetime.datetime.now()
		
		# the reason for getting previous month is so we actually have data for the report,
		# this script will run on the first day of the current month via crontab, so we need
		# to get data from the previous month (since there is no data yet for the current month)
		monthInt = now.month - 1;
		month = str(monthInt);
		year = str(now.year);
		lastDayOfMonth = str(calendar.monthrange(now.year, monthInt)[1])
		
		link = '' 
		link += month + '-01-' + year + '&endDate=' + month + '-' + lastDayOfMonth + '-' + year
		
		
		# create message container
		msg = MIMEMultipart('alternative')
		# timestamp in subject line, NOTE: based on server time
		ts = time.time()
		st = datetime.datetime.fromtimestamp(ts).strftime('%Y-%m-%d %H:%M:%S')
		
		#create message subject line and setting sender and recipient emails
		msg['Subject'] = "Gunter Hans " + calendar.month_name[monthInt] +" Report at [" + st + "]"
		msg['From'] = "gunterhansreporting@gmail.com"
		msg['To'] = "gunterhansreporting@gmail.com"
		
		# if not sent as html, the link will be in a file instead of as text
		# (for gmail at least)
		msg.attach(MIMEText(link, 'html'))
		
		mail.sendmail(msg['From'], msg['To'], msg.as_string())
		mail.close()
		# print the generated link
		# if this is not printed to the console, the email probably diden't send 
		print link