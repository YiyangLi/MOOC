(*HW2, Programming Languages @ Coursera / UWash, by Yiyang Li, from Chicago*)
(* Hw2Provided *)

(* Dan Grossman, Coursera PL, HW2 Provided Code *)

(* if you use this function to compare two strings (returns true if the same
   string), then you avoid several of the functions in problem 1 having
   polymorphic types that may be confusing *)
fun same_string(s1 : string, s2 : string) =
    s1 = s2

(* put your solutions for problem 1 here *)

(* you may assume that Num is always used with values 2, 3, ..., 10
   though it will not really come up *)
datatype suit = Clubs | Diamonds | Hearts | Spades
datatype rank = Jack | Queen | King | Ace | Num of int 
type card = suit * rank

datatype color = Red | Black
datatype move = Discard of card | Draw 

exception IllegalMove

exception InvalidCard

(* put your solutions for problem 2 here *)

(* End of Hw2Provided*)

(* Q1 first-name substitutions *)
exception MoreThanOneOccurrence

fun in_strings(matchee: string, matchers: string list) =
    case matchers of
	[] => false
      | (hd::tl) => same_string(matchee, hd) orelse in_strings(matchee, tl)
    
fun list_or_empty listOption =
    case listOption of
	NONE => []
      | SOME [] => [] 
      | SOME (head::tail) => head::tail;

fun appendList(xs, ys) =
    case xs of
	[] => ys
      | x::xs' => x :: appendList(xs',ys)

fun composeName(firstNames: string list, name: {first:string, middle:string, last:string}) =
    let
	val {first, middle = m, last = l} = name
        fun tailRec(lst, acc) =
	    case lst of
		[] => acc
	      | x::xs => tailRec(xs, acc @ [{first=x, middle=m, last=l}])
    in
	tailRec(firstNames, [name])
    end


(* 1a *)
fun all_except_option(matchee: string, matchers: string list) =
    case matchers of
	[] => NONE
      | (head::tail) => case (same_string(matchee, head), in_strings(matchee, tail)) of	   
			    (true, false) => SOME tail
			  | (false, true) => SOME (head :: list_or_empty(all_except_option(matchee, tail)))
			  | (false, false) => NONE
			  | (true, true) =>  raise MoreThanOneOccurrence

(* 1b *)
fun get_substitutions1(matchers: string list list, matchee: string) = 
    case matchers of
	[] => []
     | (headList::tailList) => list_or_empty(all_except_option(matchee, headList)) @ get_substitutions1(tailList, matchee)

(* 1c *)
fun get_substitutions2(matchers: string list list, matchee: string) = 
    let 
	fun tailRec(lst, acc) =
	    case lst of
		[] => acc
	     | x::xs => tailRec(xs, acc @ list_or_empty(all_except_option(matchee, x))) 
    in
	tailRec(matchers, [])
    end

(* 1d *)
fun similar_names(matchers: string list list, name: {first:string, middle:string, last:string}) =
    let
	val {first = firstName, middle, last} = name 
	val nameResult = composeName(get_substitutions2(matchers, firstName), name)
    in
	nameResult
    end

(* Q2 A solitaire card game  *)

fun same_card(c1 : card, c2 : card) =
    c1 = c2

fun in_cards(c1: card, cards: card list) =
    case cards of
	[] => false
      | (hd::tl) => same_card(c1, hd) orelse in_cards(c1, tl)

datatype status = Stop | Continue

(* 2a *)

fun validateCard(c: card) =
    case c of
	(_, Num i) => if (i > 10) orelse (i < 2) 
		      then raise InvalidCard
		      else c
      | _ => c 

fun card_color(c: card) = 
    case validateCard(c) of
	(Spades, _) => Black
      | (Clubs, _) => Black
      | (Diamonds, _) => Red
      | (Hearts, _) => Red

(* 2b *)
fun card_value(c: card) =
    case validateCard(c) of
	(_, Ace) => 11
      | (_, Num i) => i
      | _ => 10


(* 2c *)
fun remove_card(cards: card list, c: card, e: exn) =
    case cards of
	[] => raise e
      | (head::tail) => case (same_card(c, head), in_cards(c, tail)) of	   
			    (true, _) => tail
			  | (false, true) => head::remove_card(tail, c, e)
			  | (false, false) => raise e 

(* 2d  *)
fun same_color(c1 : card, c2 : card) = 
    case (card_color(c1),card_color(c2)) of
	(Black, Black) => true
      | (Red, Red) => true
      | _ => false


fun all_same_color(cards: card list) =
    case cards of
	[] => true
      | c::[] => validateCard(c) = c 
      | head::(neck::rest) => same_color(validateCard(head), validateCard(neck)) andalso all_same_color(neck::rest) 

(* 2e *)
fun sum_cards(cards: card list) =
    let
	fun sumValue(lst, acc) = case lst of
				 [] => acc
			       | head::tail => sumValue(tail, card_value(validateCard(head)) + acc)
    in
	sumValue(cards, 0)
    end

(* 2f *)
fun score(cards: card list, goal: int) = 
    let 
	val sum_of_cards = sum_cards(cards)
	val preliminary_score = if (sum_of_cards > goal)
				then 3 * (sum_of_cards - goal)
				else (goal - sum_of_cards)
    in
	if all_same_color(cards)
	then preliminary_score div 2
	else preliminary_score
    end

(* 2g *)
fun officiate(cards: card list, mv : move list, goal : int) = 
    let
	fun continue_game(restCards: card list, heldCards: card list, moves: move list) = 
	    case (restCards, heldCards, moves) of
		([], _, _) => score(heldCards, goal)
	      | (_, _, []) => score(heldCards, goal)
	      | (nextCard::tailCards, heldCards, nextMove::tailMoves) => case nextMove of
									     Draw => if sum_cards(nextCard::heldCards) > goal
										     then score(nextCard::heldCards, goal)
										     else continue_game(tailCards, nextCard::heldCards, tailMoves)
											| Discard c => continue_game(restCards, remove_card(heldCards, c, IllegalMove), tailMoves) 
    in
	continue_game(cards, [] : card list, mv)
    end
