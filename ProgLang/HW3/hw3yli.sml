(* Coursera Programming Languages, Homework 3, Provided Code *)

exception NoAnswer

datatype pattern = Wildcard
		 | Variable of string
		 | UnitP
		 | ConstP of int
		 | TupleP of pattern list
		 | ConstructorP of string * pattern

datatype valu = Const of int
	      | Unit
	      | Tuple of valu list
	      | Constructor of string * valu

fun g f1 f2 p =
    let 
	val r = g f1 f2 
    in
	case p of
	    Wildcard          => f1 ()
	  | Variable x        => f2 x
	  | TupleP ps         => List.foldl (fn (p,i) => (r p) + i) 0 ps
	  | ConstructorP(_,p) => r p
	  | _                 => 0
    end

(**** for the challenge problem only ****)

datatype typ = Anything
	     | UnitT
	     | IntT
	     | TupleT of typ list
	     | Datatype of string

(**** you can put all your code here ****)
(*Homework 3 of Programming Languages, Coursera, UWash. Done by Yiyang Li, from Chicago*)
(*Q1*)
fun only_capitals xs = List.filter (fn s =>
				       if String.size s = 0
				       then false
				       else Char.isUpper(String.sub(s,0))) xs;

(*Q2*)
fun longest_string1 xs = 
    foldl (fn (s,longer) => if String.size s > String.size longer
			    then s
			    else longer) "" xs
(*Q3*)
fun longest_string2 xs = 
    foldl (fn (s,longer) => if String.size s >= String.size longer
			    then s
			    else longer) "" xs

(*Q4*)
fun longest_string_helper comparator xs = 
     foldl (fn (s,longer) => if comparator(String.size s, String.size longer)
			    then s
			    else longer) "" xs
val longest_string3 = longest_string_helper (fn (x,y) => x > y)
val longest_string4 = longest_string_helper (fn (x,y) => x >= y)

(*Q5*)
val longest_capitalized = longest_string3 o only_capitals

(* Q6 *)
val rev_string = String.implode o List.rev o String.explode

(* Q7 *)
fun first_answer f xs =
    case xs of
	[] => raise NoAnswer
     | x::xs' => case f x of
		     NONE => first_answer f xs'
		  | SOME x => x

(* Q8 *)
fun list_or_empty listOption =
    case listOption of
	NONE => []
      | SOME lst => lst 

fun all_answers f xs = 
    case xs of
	[] => NONE
      | x::xs' => case f x of
		      NONE => NONE
		    | SOME [] => SOME (list_or_empty(all_answers f xs')) 
		    | SOME x => SOME (x @ list_or_empty(all_answers f xs'))

(* Q9 *)
val count_wildcards = g (fn ()=> 1) (fn s => 0)
val count_wild_and_variable_lengths = g (fn ()=> 1) (fn s => String.size s)
fun count_some_var (str, p) = g (fn () => 0) 
				(fn s => if (s = str) 
					 then 1 
					 else 0) 
				p

(* Q10 *)
fun getVarNames p =
	case p of
	    TupleP ps  => List.foldl (fn (p,lst) => getVarNames p @ lst) [] ps
	  | Variable x => [x]
	  | ConstructorP(_, p) => getVarNames p 
	  | _          => []

fun noDup [] = false
  | noDup (x::xs) = (List.exists (fn y : string => y=x ) xs) orelse (noDup xs)

val check_pat = not o noDup o getVarNames

(* Q11 *)

fun isMatched (v, p)= 
    case (v, p) of 
	(Unit, UnitP) => true
      | (Const x, ConstP y) => (x = y) 
      | (Tuple vs, TupleP ps) => List.length ps = List.length vs andalso 
				 let 
				     val pair = ListPair.zip(vs, ps)
				 in
				     List.foldl (fn (pv,acc) => (isMatched pv) andalso acc) true pair
				 end
      | (Constructor(s1,v), ConstructorP(s2,p)) => (s1 = s2) 
      | (v, Variable s) => true
      | (_, Wildcard) => true 
      | _ => false


fun match (v,p) =
    if isMatched(v, p) 
    then 
	case (v, p) of
	    (Constructor(_,v1), ConstructorP(_,p1)) => match(v1, p1)
	  | (Tuple vs, TupleP ps) => let val pair = ListPair.zip(vs, ps)
				     in
					 all_answers match pair
				     end
	  | (v, Variable s) => SOME [(s,v)]
	  | _ => SOME [] 
    else
	NONE    
(* Q12 *)
fun first_match v ps = SOME (first_answer (fn p=> match(v, p)) ps) handle NoAnswer => NONE 
