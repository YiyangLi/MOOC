(* Yiyang Li from Chicago, Coursera PL, HW1 Solution Code 
   Auto-grading score: 104/100
   Peer-assessment:    59/65
*)

val CONST_MONTHS = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];

val CONST_DAYS = [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31];

(* Q1 define is_older that takes 2 dates and evaluates if the first comes before the second.*)

fun is_equal(first: int*int*int, second: int*int*int) =
    ((#1 first) = (#1 second)) andalso ((#2 first) = (#2 second)) andalso ((#3 first) = (#3 second))

fun is_older(first: int*int*int, second: int*int*int) = 
    if (#1 first = #1 second)
    then if (#2 first = #2 second)
	 then if (#3 first = #3 second)
	      then false
	      else (#3 first < #3 second)
	 else (#2 first < #2 second)
    else (#1 first < #1 second)

(* Q2 given a list of dates and a given month, return the # of dates that are in the month *)

fun is_in_month(dateMonth: int, month: int) =
    (dateMonth = month)

fun is_in_months(dateMonth: int, months: int list) =
   if null months
   then false
   else
       if is_in_month(dateMonth, hd months)
       then true
       else is_in_months(dateMonth, tl months)

fun get_Months(dates: (int*int*int) list) =
    if null dates
    then []
    else #2 (hd dates) :: get_Months(tl dates);

fun months_in_months(dateMonths: int list, months: int list) =
    if null dateMonths
    then 0
    else 
	let 
	    val isInMonths = if is_in_months((hd dateMonths), months) 
			     then 1
			     else 0
	in 
	    isInMonths + months_in_months((tl dateMonths), months)
	end

fun number_in_month(dates: (int*int*int) list, month: int) =
    if null dates
    then 0
    else months_in_months(get_Months(dates), month :: []);

(* Q3 expand the Q2, now take a list of months *)

fun number_in_months(dates: (int*int*int) list, months: int list) =
    if (null dates) orelse (null months)
    then 0
    else months_in_months(get_Months(dates), months);

(* Q4 expand Q3, now return the list holding the dates by the given month *)

fun dates_in_month(dates: (int*int*int) list, month: int) =
    if null dates
    then []
    else 
	let 
	    val date = hd dates;
	in
	    if is_in_month(#2 date, month) 
	    then date :: dates_in_month(tl dates, month)
	    else dates_in_month(tl dates, month)
	end

(* Q5 expand Q4, now return the list holding the dates by the given months *)

fun dates_in_months(dates: (int*int*int) list, months: int list) =
    if (null dates) orelse (null months)
    then []
    else 
	let 
	    val date = hd dates
	in
	    if is_in_months(#2 date, months) 
	    then date :: dates_in_months(tl dates, months)
	    else dates_in_months(tl dates, months)
	end

(* Q6 get the nth string, where the first one is 1st*)
fun get_nth(listOfStrings: string list, index: int) = 
    if (null listOfStrings) orelse (index <= 0)
    then ""
    else 
	if (not (null listOfStrings) andalso (index = 1))
	then hd listOfStrings
	else get_nth(tl listOfStrings, index - 1)

(* Q7 translate date to string*)

fun date_to_string(date: (int*int*int)) =
    get_nth(CONST_MONTHS, #2 date) ^ " " ^ Int.toString(#3 date) ^ ", " ^ Int.toString(#1 date);

(* Q8 number_before_reaching_sum find the nth number that reach the sum, output n*)

fun number_before_reaching_sum(sum: int, intList: int list) = 
    if (null intList) orelse (sum - (hd intList) <= 0)
    then 0
    else 1 + number_before_reaching_sum(sum - (hd intList), tl intList);

(* Q9 what_month return the month given the day of year*)

fun what_month(day: int) =
    1 + number_before_reaching_sum(day, CONST_DAYS);

(* Q10 return the months of each day between [day1, day2]*)
fun month_range(day1: int, day2: int) =
    if (day1 > day2)
    then []
    else what_month(day1) :: month_range(day1 + 1, day2); 
		       
(* Q11 return the oldest date from a list of dates, if it's an empty list, return NONE else SOME d*)
fun oldest(dates: (int*int*int) list) =
    if null dates
    then NONE
    else
	let 
	    fun oldest_nonempty (dates: (int*int*int) list) =
	    if null (tl dates)
	    then hd dates
	    else
		let 
		    val tl_ans = oldest_nonempty(tl dates)
		in 
		    if is_older(hd dates, tl_ans)
		    then hd dates
		    else tl_ans
		end
	in
	    SOME (oldest_nonempty dates)
	end

(* Q12 didn't quite understand it, I did reuse codes in problem 3 and 5. *)


fun number_in_months_challenge(dates: (int*int*int) list, months: int list) =
    if (null dates) orelse (null months)
    then 0
    else months_in_months(get_Months(dates), months);

fun dates_in_months_challenge(dates: (int*int*int) list, months: int list) =
    if (null dates) orelse (null months)
    then []
    else 
	let 
	    val date = hd dates
	in
	    if is_in_months(#2 date, months) 
	    then date :: dates_in_months(tl dates, months)
	    else dates_in_months(tl dates, months)
	end

(* Q13 reasonable date, validate the input date *)
fun get_days(month: int, days: int list) = 
    if (month = 1)
    then hd days
    else 
	let val tl_ans = get_days(month - 1, tl days)
	in 
	    tl_ans
	end

fun get_day_of_month(date: (int*int*int)) =
    let
	val LEAP1 = ((#1 date) mod 100 = 0) andalso ((#1 date) mod 400 = 0)
	val LEAP2 = ((#1 date) mod 4 = 0) andalso ((#1 date) mod 100 <> 0)
    in
	if (LEAP1 orelse LEAP2) andalso (#2 date = 2)
	then 29
	else get_days(#2 date, CONST_DAYS)
    end

fun reasonable_date(date: (int*int*int)) =
    if (#1 date <=0 ) orelse (#2 date <= 0) orelse (#3 date <=0) orelse (#3 date > 31) orelse (#2 date > 12)
    then false
    else (#3 date <= get_day_of_month(date))

(* End of the HW1 Solution Code, done by Yiyang Li from Chicago *)
