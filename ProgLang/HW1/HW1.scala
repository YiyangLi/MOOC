package greeter

/**
 * @author yyli
 */
class hw1 {
  val MONTHS = List("January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December");
                                                  //> MONTHS  : List[String] = List(January, February, March, April, May, June, Ju
                                                  //| ly, August, September, October, November, December)
  val DAYS = List(31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31)
                                                  //> DAYS  : List[Int] = List(31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31)
  def isDateEqual(first: (Int, Int, Int), second: (Int, Int, Int)) : Boolean =
    (first._1 == second._1 && first._2 == second._2 && first._3 == second._3)
                                                  //> isDateEqual: (first: (Int, Int, Int), second: (Int, Int, Int))Boolean
  def is_older(first: (Int, Int, Int), second: (Int, Int, Int)) : Boolean =
    if (first._1 == second._1)
      if (first._2 == second._2)
        if (first._3 == second._3)
          false
          else (first._3 < second._3)
      else (first._2 < second._2)
    else (first._1 < second._1)               //> is_older: (first: (Int, Int, Int), second: (Int, Int, Int))Boolean
  def number_in_month(dates: List[(Int, Int, Int)], month: Int) : Int = dates match {
    case Nil => 0
    case date :: tail => (if (date._2 == month) 1 else 0) + number_in_month(tail, month)
  }                                               //> number_in_month: (dates: List[(Int, Int, Int)], month: Int)Int
  
  def number_in_months(dates: List[(Int, Int, Int)], months: List[Int]) : Int = dates match {
    case Nil => 0
    case date :: tail => (if (months.contains(date._2)) 1 else 0) + number_in_months(tail,months)
  }                                               //> number_in_months: (dates: List[(Int, Int, Int)], months: List[Int])Int
  
  val Dates : List[(Int, Int, Int)] = List((1,2,3))
                                                  //> Dates  : List[(Int, Int, Int)] = List((1,2,3))
  def dates_in_month(dates: List[(Int, Int, Int)], month: Int) : List[(Int, Int, Int)] = dates match {
    case Nil => List.empty
    case date :: tail => if (date._2 == month) date :: dates_in_month(tail,month) else dates_in_month(tail,month)
  }                                               //> dates_in_month: (dates: List[(Int, Int, Int)], month: Int)List[(Int, Int, I
                                                  //| nt)]
  def dates_in_months(dates: List[(Int, Int, Int)], months: List[Int]) : List[(Int, Int, Int)] = dates match {
    case Nil => List.empty
    case date :: tail => if (months.contains(date._2)) date :: dates_in_months(tail,months) else dates_in_months(tail,months)
  }                                               //> dates_in_months: (dates: List[(Int, Int, Int)], months: List[Int])List[(Int
                                                  //| , Int, Int)]
  def get_nth(listOfStrings: List[String], index: Int) : String = {
    if (listOfStrings.isEmpty || index <= 0)
      ""
    else
      if ((!listOfStrings.isEmpty) && (index == 1))
        listOfStrings.head
      else
        get_nth(listOfStrings.tail, index - 1)
  }                                               //> get_nth: (listOfStrings: List[String], index: Int)String
  def date_to_string(date: (Int, Int, Int)) : String = {
    get_nth(MONTHS, date._2) + " " + date._3.toString + ", " + date._1.toString
  }                                               //> date_to_string: (date: (Int, Int, Int))String
  def number_before_reaching_sum(sum: Int, lst: List[Int]) : Int = {
    if (lst.isEmpty)
      throw new java.lang.IllegalArgumentException("Sum of the elements can't reach the sum")
    else
      if (sum - lst.head > 0)
        1 + number_before_reaching_sum(sum - lst.head, lst.tail)
      else
        0
  }                                               //> number_before_reaching_sum: (sum: Int, lst: List[Int])Int
  def what_month(day: Int) : Int = {
    1 + number_before_reaching_sum(day, DAYS)
  }                                               //> what_month: (day: Int)Int
  def month_range(day1: Int, day2: Int) : List[Int] = {
    if (day1 <= day2)
      what_month(day1) :: month_range(day1 + 1, day2)
    else
      Nil
  }                                               //> month_range: (day1: Int, day2: Int)List[Int]
  def oldest(dates: List[(Int, Int, Int)]) : Option[(Int, Int, Int)] = dates match{
    case Nil => None
    case date1 :: tail => oldest(tail) match { case None => Some(date1)
                                               case Some(tailOldest) => (if (is_older(date1, tailOldest))
                                                                            Some(date1)
                                                                         else
                                                                            Some(tailOldest))}
  }
  
  def get_day(month: Int, days: List[Int]) : Int = {
    if (month == 1)
      days.head
    else
      get_day(month - 1, days.tail)
  }                                               //> get_day: (month: Int, days: List[Int])Int
  
  def get_day_of_month(date: (Int, Int, Int)) : Int = {
    val LEAP1 = (date._1 % 100 == 0 && date._1 % 400 == 0)
    val LEAP2 = (date._1 % 4 == 0 && date._1 % 100 != 0)
    if ((LEAP1 || LEAP2) && date._2 == 2)
      29
    else
      get_day(date._2, DAYS)
  }                                               //> get_day_of_month: (date: (Int, Int, Int))Int
  
  def reasonable_date(date: (Int, Int, Int)) : Boolean = {
    if (date._1 <= 0 || date._2 <= 0 | date._3 <= 0 || date._3 > 31 || date._2 >12)
      false
    else (date._3 <= get_day_of_month(date))
  }                                               //> reasonable_date: (date: (Int, Int, Int))Boolean
}