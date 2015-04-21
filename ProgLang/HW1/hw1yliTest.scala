package greeter


/**
 * @author yyli
 */
object main extends App {
  val HW1 = new greeter.hw1
  println("Tests for HW1");
  println("Test1_1: " + (HW1.is_older((2007, 7, 7), (2008, 7, 7)) == true).toString);
  println("Test1_2: " + (HW1.is_older((2007, 7, 7), (2007, 8, 7)) == true).toString);
  println("Test1_3: " + (HW1.is_older((2007, 7, 7), (2007, 7, 8)) == true).toString);
  println("Test1_4: " + (HW1.is_older((2007, 7, 7), (2007, 7, 7)) == false).toString);
  println("Test1_5: " + (HW1.is_older((2008, 7, 7), (2007, 7, 7)) == false).toString);
  println("Test1_6: " + (HW1.is_older((2007, 8, 7), (2007, 7, 7)) == false).toString);
  println("Test1_7: " + (HW1.is_older((2007, 7, 8), (2007, 7, 7)) == false).toString);
  
  println("Test2_1: " + (HW1.number_in_month(List((1,2,3), (4,5,6)), 2) == 1).toString);
  println("Test2_2: " + (HW1.number_in_month(List((1,2,3), (4,2,6)), 2) == 2).toString);
  println("Test2_3: " + (HW1.number_in_month(List((1,2,3), (4,2,6)), 4) == 0).toString);
  println("Test2_4: " + (HW1.number_in_month(List((1,2,3), (4,5,6)), 3) == 0).toString);
  println("Test2_5: " + (HW1.number_in_month(List((1,2,3), (4,3,6), (3,3,3)), 3) == 2).toString);

  println("Test3_1: " + (HW1.number_in_months(List((1,2,3), (4,5,6)), List(2,3,4)) == 1).toString);
  println("Test3_2: " + (HW1.number_in_months(List((1,2,3), (4,2,6)), List(2,3,4)) == 2).toString);
  println("Test3_3: " + (HW1.number_in_months(List((1,2,3), (4,2,6)), List(3,4)) == 0).toString);
  println("Test3_4: " + (HW1.number_in_months(List((1,2,3), (4,5,6)), List(3)) == 0).toString);
  println("Test4_5: " + (HW1.number_in_months(List((1,2,3), (4,3,6), (3,3,3)), List(2, 3)) == 3).toString);
  
  println("Test4_1: " + (HW1.dates_in_month(List((1,2,3), (4,5,6)), 2) == List((1,2,3))).toString);
  println("Test4_2: " + (HW1.dates_in_month(List((1,2,3), (4,2,6)), 2) == List((1,2,3), (4,2,6))).toString);
  println("Test4_3: " + (HW1.dates_in_month(List((1,2,3), (4,2,6)), 4) == Nil).toString);
  println("Test4_4: " + (HW1.dates_in_month(List((1,2,3), (4,5,6)), 3) == Nil).toString);
  println("Test4_5: " + (HW1.dates_in_month(List((1,2,3), (4,3,6), (3,3,3)), 3) == List((4,3,6), (3, 3, 3))).toString);
  
  println("Test5_1: " + (HW1.dates_in_months(List((1,2,3), (4,5,6)), List(2,3,4)) == List((1,2,3))).toString);
  println("Test5_2: " + (HW1.dates_in_months(List((1,2,3), (4,2,6)), List(2,3,4)) == List((1,2,3), (4,2,6))).toString);
  println("Test5_3: " + (HW1.dates_in_months(List((1,2,3), (4,2,6)), List(3,4)) == Nil).toString);
  println("Test5_4: " + (HW1.dates_in_months(List((1,2,3), (4,5,6)), List(3)) == Nil).toString);
  println("Test5_5: " + (HW1.dates_in_months(List((1,2,3), (4,3,6), (3,3,3)), List(2,3)) == List((1,2,3), (4,3,6), (3,3,3))).toString);
  
  val MONTHS = List("January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December");
  
  println("Test6_1: " + (HW1.get_nth(MONTHS, 1) == "January").toString);
  println("Test6_2: " + (HW1.get_nth(MONTHS, 7) == "July").toString);
  println("Test6_3: " + (HW1.get_nth(MONTHS, 12) == "December").toString);
  println("Test6_4: " + (HW1.get_nth(MONTHS, 13) == "").toString);
  
  println("Test7_1: " + (HW1.date_to_string((2013, 1, 20)) == "January 20, 2013").toString);
  println("Test7_2: " + (HW1.date_to_string((1990, 3, 20)) == "March 20, 1990").toString);
  println("Test7_3: " + (HW1.date_to_string((1988, 8, 20)) == "August 20, 1988").toString);
  
  
  println("Test8_1: " + (HW1.number_before_reaching_sum(5, List(1, 2, 3, 4)) == 2).toString);
  println("Test8_2: " + (HW1.number_before_reaching_sum(6, List(1, 2, 3, 4)) == 2).toString);
  println("Test8_3: " + (HW1.number_before_reaching_sum(7, List(1, 2, 3, 4)) == 3).toString);
  println("Test8_4: " + (HW1.number_before_reaching_sum(5, List(4, 2, 1, 3)) == 1).toString);
  println("Test8_5: " + (HW1.number_before_reaching_sum(6, List(4, 2, 1, 3)) == 1).toString);
  println("Test8_6: " + (HW1.number_before_reaching_sum(40, List(1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1)) == 39).toString);
  println("Test8_7: " + (HW1.number_before_reaching_sum(40, List(1,1,1,1,1,1,1,1,1,100,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1)) == 9).toString);
  
  println("Test9_1: " + (HW1.what_month(6) == 1).toString);
  println("Test9_2: " + (HW1.what_month(365) == 12).toString);
  println("Test9_3: " + (HW1.what_month(59) == 2).toString);
  println("Test9_4: " + (HW1.what_month(184) == 7).toString);
  
  println("Test10_1: " + (HW1.month_range(10, 2) == Nil).toString);
  println("Test10_2: " + (HW1.month_range(1,1) == List(1)).toString);
  println("Test10_3: " + (HW1.month_range(31,32) == List(1,2)).toString);
  println("Test10_4: " + (HW1.month_range(1,32) == List(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2)).toString);
  
  println("Test11_1: " + (HW1.oldest(Nil) == None).toString);
  println("Test11_2: " + (HW1.oldest(List((2014,10,13))) == Some(2014,10,13)).toString);
  println("Test11_3: " + (HW1.oldest(List((2014,10,13), (2014,10,14), (2014,10,15), (2014,10,16))) == Some(2014,10,13)).toString);
  
  println("Test13_1: " + (HW1.reasonable_date((1990, 2, 29)) == false).toString)
  println("Test13_2: " + (HW1.reasonable_date((1990, 2, 29)) == false).toString)
  println("Test13_3: " + (HW1.reasonable_date((0, 2, 29)) == false).toString)
  println("Test13_4: " + (HW1.reasonable_date((1998, 4, 31)) == false).toString)
  println("Test13_5: " + (HW1.reasonable_date((1998, 4, -1)) == false).toString)
  println("Test13_6: " + (HW1.reasonable_date((1900, 2, 29)) == false).toString)
 
}