(* Homework1 Simple Test provided by Yiyang Li *)

val test1_1 = is_older((2007, 7, 7), (2008, 7, 7)) = true
val test1_2 = is_older((2007, 7, 7), (2007, 8, 7)) = true
val test1_3 = is_older((2007, 7, 7), (2007, 7, 8)) = true
val test1_4 = is_older((2007, 7, 7), (2007, 7, 7)) = false
val test1_5 = is_older((2008, 7, 7), (2007, 7, 7)) = false
val test1_6 = is_older((2007, 8, 7), (2007, 7, 7)) = false
val test1_7 = is_older((2007, 7, 7), (2007, 7, 1)) = false

val test2_1 = number_in_month([(1,2,3), (4,5,6)], 2) = 1
val test2_2 = number_in_month([(1,2,3), (4,2,6)], 2) = 2
val test2_3 = number_in_month([(1,2,3), (4,2,6)], 4) = 0
val test2_4 = number_in_month([(1,2,3), (4,5,6)], 3) = 0
val test2_5 = number_in_month([(1,2,3), (4,3,6), (3, 3, 3)], 3) = 2

val test3_1 = number_in_months([(1,2,3), (4,5,6)], [2,3,4]) = 1
val test3_2 = number_in_months([(1,2,3), (4,2,6)], [2,3,4]) = 2
val test3_3 = number_in_months([(1,2,3), (4,2,6)], [3,4]) = 0
val test3_4 = number_in_months([(1,2,3), (4,5,6)], [3]) = 0
val test3_5 = number_in_months([(1,2,3), (4,3,6), (3, 3, 3)], [2,3]) = 3

val test4_1 = dates_in_month([(1,2,3), (4,5,6)], 2) = [(1,2,3)]
val test4_2 = dates_in_month([(1,2,3), (4,2,6)], 2) = [(1,2,3), (4,2,6)]
val test4_3 = dates_in_month([(1,2,3), (4,2,6)], 4) = []
val test4_4 = dates_in_month([(1,2,3), (4,5,6)], 3) = []
val test4_5 = dates_in_month([(1,2,3), (4,3,6), (3, 3, 3)], 3) = [(4,3,6), (3, 3, 3)]


val test5_1 = dates_in_months([(1,2,3), (4,5,6)], [2,3,4]) = [(1,2,3)]
val test5_2 = dates_in_months([(1,2,3), (4,2,6)], [2,3,4]) = [(1,2,3), (4,2,6)]
val test5_3 = dates_in_months([(1,2,3), (4,2,6)], [3,4]) = []
val test5_4 = dates_in_months([(1,2,3), (4,5,6)], [3]) = []
val test5_5 = dates_in_months([(1,2,3), (4,3,6), (3, 3, 3)], [2,3]) = [(1,2,3), (4,3,6), (3, 3, 3)]
 
val test6_1 = get_nth(["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"], 1) = "January";
val test6_2 = get_nth(["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"], 7) = "July";
val test6_3 = get_nth(["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"], 12) = "December";
val test6_4 = get_nth(["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"], 13) = "";

val test7_1 = date_to_string((2013, 1, 20)) = "January 20, 2013";
val test7_2 = date_to_string((1990, 3, 20)) = "March 20, 1990";
val test7_3 = date_to_string((1988, 8, 20)) = "August 20, 1988";

val test8_1 = number_before_reaching_sum(5, [1,2,3,4]) = 2;
val test8_2 = number_before_reaching_sum(6, [1,2,3,4]) = 2;
val test8_3 = number_before_reaching_sum(7, [1,2,3,4]) = 3;
val test8_4 = number_before_reaching_sum(5, [4,2,1,3]) = 1;
val test8_5 = number_before_reaching_sum(6, [4,2,1,3]) = 1;
val test8_6 = number_before_reaching_sum(40, [1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1]) = 39;
val test8_7 = number_before_reaching_sum(40, [1,1,1,1,1,1,1,1,1,100,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1]) = 9;

val test9_1 = what_month(1) = 1;
val test9_2 = what_month(365) = 12;
val test9_3 = what_month(59) = 2;
val test9_4 = what_month(184) = 7;

val test10_1 = month_range(10, 2) = [];
val test10_2 = month_range(1, 1) = [1];
val test10_3 = month_range(31, 32) = [1,2];
val test10_4 = month_range(1, 32) = [1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2];

val test11_1 = oldest([]) = NONE;
val test11_2 = oldest([(2014,10,13)]) = SOME (2014, 10, 13);
val test11_3 = oldest([(2014,10,13), (2014,10,14), (2014,10,15), (2014,10,16)]) = SOME (2014, 10, 13);

val test13_1 = reasonable_date((1990, 2, 29)) = false;
val test13_2 = reasonable_date((2000, 2, 29)) = true;
val test13_3 = reasonable_date((0, 2, 29)) = false;
val test13_4 = reasonable_date((1998, 4, 31)) = false;
val test13_5 = reasonable_date((1998, 4, ~1)) = false;
val test13_6 = reasonable_date((1900, 2, 29)) = false;

(* End of testing*)  
