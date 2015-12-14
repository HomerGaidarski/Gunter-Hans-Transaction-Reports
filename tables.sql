CREATE TABLE gh_transaction (
 
                id bigint,
                tran_time time,
    tran_date date,
    subtotal double (10, 2),
                tip_amount double (10, 2),
    discount double (10, 2),
    tax double (10, 2),
    total double (10, 2),
    emp_id int,
   
    PRIMARY KEY (id),
    FOREIGN KEY (emp_id) REFERENCES gh_employee(emp_id)
 
) ENGINE=INNODB;
 
CREATE INDEX transaction_index ON gh_transaction(tran_date, tran_time)
 
CREATE TABLE `user` (
  `username` varchar(20) NOT NULL,
  `salt` varchar(20) DEFAULT NULL,
  `hashed_password` varchar(256) DEFAULT NULL,
  `is_admin` tinyint(1) DEFAULT NULL,
  PRIMARY KEY (`username`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8
 
CREATE TABLE `gh_employee` (
  `fname` varchar(20) DEFAULT NULL,
  `lname` varchar(20) DEFAULT NULL,
  `emp_id` int(11) NOT NULL AUTO_INCREMENT,
  PRIMARY KEY (`emp_id`)
) ENGINE=InnoDB AUTO_INCREMENT=24 DEFAULT CHARSET=utf8
