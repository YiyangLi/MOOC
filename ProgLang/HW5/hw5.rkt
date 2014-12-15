;; Programming Languages, Homework 5

#lang racket
(provide (all-defined-out)) ;; so we can put tests in a second file

;; definition of structures for MUPL programs - Do NOT change
(struct var  (string) #:transparent)  ;; a variable, e.g., (var "foo")
(struct int  (num)    #:transparent)  ;; a constant number, e.g., (int 17)
(struct add  (e1 e2)  #:transparent)  ;; add two expressions
(struct ifgreater (e1 e2 e3 e4)    #:transparent) ;; if e1 > e2 then e3 else e4
(struct fun  (nameopt formal body) #:transparent) ;; a recursive(?) 1-argument function
(struct call (funexp actual)       #:transparent) ;; function call
(struct mlet (var e body) #:transparent) ;; a local binding (let var = e in body) 
(struct apair (e1 e2)     #:transparent) ;; make a new pair
(struct fst  (e)    #:transparent) ;; get first part of a pair
(struct snd  (e)    #:transparent) ;; get second part of a pair
(struct aunit ()    #:transparent) ;; unit value -- good for ending a list
(struct isaunit (e) #:transparent) ;; evaluate to 1 if e is unit else 0

;; a closure is not in "source" programs; it is what functions evaluate to
(struct closure (env fun) #:transparent) 

;; Problem 1

(define (racketlist->mupllist rlist)
  (if (null? rlist)
      (aunit)
      (apair (car rlist) (racketlist->mupllist (cdr rlist)))))

;; Problem 2
(define (mupllist->racketlist mlist)
  (if (aunit? mlist)
      null
      (cons (apair-e1 mlist) (mupllist->racketlist (apair-e2 mlist)))))

;; lookup a variable in an environment
;; Do NOT change this function
(define (envlookup env str)
  (cond [(null? env) (error "unbound variable during evaluation" str)]
        [(equal? (car (car env)) str) (cdr (car env))]
        [#t (envlookup (cdr env) str)]))

;; Do NOT change the two cases given to you.  
;; DO add more cases for other kinds of MUPL expressions.
;; We will test eval-under-env by calling it directly even though
;; "in real life" it would be a helper function of eval-exp.
(define (eval-under-env e env)
  (cond [(var? e) 
         (envlookup env (var-string e))]
        [(add? e) 
         (let ([v1 (eval-under-env (add-e1 e) env)]
               [v2 (eval-under-env (add-e2 e) env)])
           (if (and (int? v1)
                    (int? v2))
               (int (+ (int-num v1) 
                       (int-num v2)))
               (error "MUPL addition applied to non-number")))]
        [(ifgreater? e) 
         (let ([v1 (eval-under-env (ifgreater-e1 e) env)]
               [v2 (eval-under-env (ifgreater-e2 e) env)])
           (if (and (int? v1)
                    (int? v2))
               (if (> (int-num v1) (int-num v2))
                   (eval-under-env (ifgreater-e3 e) env)
                   (eval-under-env (ifgreater-e4 e) env))
               (error "MUPL addition applied to non-number")))]
        [(apair? e)
         (let ([v1 (eval-under-env (apair-e1 e) env)]
               [v2 (eval-under-env (apair-e2 e) env)])
           (apair v1 v2))]
        [(fst? e)
         (let ([v (eval-under-env (fst-e e) env)])
           (if (apair? v)
               (apair-e1 v)
               (error "MUPL fst applied to non-apair")))]
        [(snd? e)
         (let ([v (eval-under-env (snd-e e) env)])
           (if (apair? v)
               (apair-e2 v)
               (error "MUPL snd applied to non-apair")))]
        [(mlet? e)  
           (let* ([exp (eval-under-env (mlet-e e) env)]
                  [env2 (append (list (cons (mlet-var e) exp)) env)])
             (eval-under-env (mlet-body e) env2))]
        [(fun? e) (closure env e)]
        [(call? e) (let ([func (eval-under-env (call-funexp e) env)]
                         [args (eval-under-env (call-actual e) env)])
                     (if (closure? func)
                         (let* ([fun1 (closure-fun func)]
                                [envr (closure-env func)]
                                [name (cons (fun-nameopt fun1) func)]
                                [arg (cons (fun-formal fun1) args)])
                           (eval-under-env (fun-body fun1) 
                                           (if (car name)
                                               (cons arg (cons name envr)) ;;strip off
                                               (cons arg envr)))) ;;strip off
                         (error "MUPL call applied to non-closure")))]  
        [(isaunit? e)
         (if (aunit? (eval-under-env (isaunit-e e) env))
             (int 1)
             (int 0))] ;; evaluate to 1 if e is unit else 0
        [(int? e) e] ;; a constant number, e.g., (int 17)   
        [(aunit? e) e]
        [(closure? e) e]
        [#t (error (format "bad MUPL expression: ~v" e))]))

;; Do NOT change
(define (eval-exp e)
  (eval-under-env e null))
        
;; Problem 3

(define (ifaunit e1 e2 e3) (ifgreater (isaunit e1) (int 0) e2 e3))

(define (mlet* lstlst e2) 
  (if (null? lstlst)
      e2
      (mlet (caar lstlst) (cdar lstlst) (mlet* (cdr lstlst) e2))))
  

(define (ifeq e1 e2 e3 e4)
  (mlet "_x" e1 (mlet "_y" e2
                      (ifgreater (add [ifgreater (var "_x") (var "_y") (int 1) (int 0)]
                                      [ifgreater (var "_y") (var "_x") (int 1) (int 0)])
                                 (int 0)
                                 e4 ;;they are different
                                 e3))))

;; Problem 4

(define mupl-map 
  (fun "HaveFun" "Y_args" (fun "Y_map" "Y_lst"
                               (ifgreater (isaunit (var "Y_lst"))
                                          (int 0)
                                          (aunit)
                                          (apair (call (var "Y_args") (fst (var "Y_lst")))
                                                 (call (var "Y_map") (snd (var "Y_lst"))))
                                          ))))
                                                 

(define mupl-mapAddN 
  (mlet "map" mupl-map
        (fun "HaveFun" "Y_arg1" (call (var "map") (fun "HaveFun" "Y_arg2" (add (var "Y_arg1") (var "Y_arg2")))))))

;; Challenge Problem

(struct fun-challenge (nameopt formal body freevars) #:transparent) ;; a recursive(?) 1-argument function

;; We will test this function directly, so it must do
;; as described in the assignment

(define (FreeVar e)
  (cond [(var? e) (set (var-string e))]
        [(int? e) (set)]
        [(add? e) (set-union (FreeVar (add-e1 e)) 
                             (FreeVar (add-e2 e)))]
        [(ifgreater? e) (set-union (FreeVar (ifgreater-e1 e)) 
                                   (FreeVar (ifgreater-e2 e)) 
                                   (FreeVar (ifgreater-e3 e)) 
                                   (FreeVar (ifgreater-e4 e)))]
        [(fun? e) (if (fun-nameopt e) 
                      (set-remove (set-remove (FreeVar (fun-body e)) (fun-formal e)) (fun-nameopt e)) 
                      (set-remove (FreeVar (fun-body e)) (fun-formal e)))]
        [(call? e) (set-union (FreeVar (call-funexp e)) 
                              (FreeVar (call-actual e)))]
        [(mlet? e) (set-union (FreeVar (mlet-e e)) 
                              (set-remove (FreeVar (mlet-body e)) (mlet-var e)))]
        [(apair? e) (set-union (FreeVar (apair-e1 e)) 
                               (FreeVar (apair-e2 e)))]
        [(fst? e) (FreeVar (fst-e e))]
        [(snd? e) (FreeVar (snd-e e))]
        [(aunit? e) (set)]
        [(isaunit? e) (FreeVar (isaunit-e e))]
        [#t (error (format "bad MUPL expression: ~v" e))]))

(define (compute-free-vars e)
  (cond [(var? e) e]
        [(int? e) e]
        [(add? e) (add (compute-free-vars (add-e1 e)) 
                       (compute-free-vars (add-e2 e)))]
        [(ifgreater? e) (ifgreater (compute-free-vars (ifgreater-e1 e)) 
                                   (compute-free-vars (ifgreater-e2 e))
                                   (compute-free-vars (ifgreater-e3 e))
                                   (compute-free-vars (ifgreater-e4 e)))]
        [(fun? e) (fun-challenge (fun-nameopt e) (fun-formal e) (compute-free-vars (fun-body e)) (FreeVar e))]
        [(call? e) (call (compute-free-vars (call-funexp e)) (compute-free-vars (call-actual e)))]
        [(mlet? e) (mlet (mlet-var e) (compute-free-vars (mlet-e e)) (compute-free-vars (mlet-body e)))]
        [(apair? e) (apair (compute-free-vars (apair-e1 e)) (compute-free-vars (apair-e2 e)))]
        [(fst? e) (fst (compute-free-vars (fst-e e)))]
        [(snd? e) (snd (compute-free-vars (snd-e e)))]
        [(aunit? e) e]
        [(isaunit? e) (isaunit (compute-free-vars (isaunit-e e)))]
        [#t (error (format "bad MUPL expression: ~v" e))]))

;; Do NOT share code with eval-under-env because that will make
;; auto-grading and peer assessment more difficult, so
;; copy most of your interpreter here and make minor changes
(define (eval-under-env-c e env) 
  (cond [(var? e) 
         (envlookup env (var-string e))] ;;same
        [(add? e) 
         (let ([v1 (eval-under-env-c (add-e1 e) env)]
               [v2 (eval-under-env-c (add-e2 e) env)])
           (if (and (int? v1)
                    (int? v2))
               (int (+ (int-num v1) 
                       (int-num v2)))
               (error "MUPL addition applied to non-number")))] ;;same
        [(ifgreater? e) 
         (let ([v1 (eval-under-env-c (ifgreater-e1 e) env)]
               [v2 (eval-under-env-c (ifgreater-e2 e) env)])
           (if (and (int? v1)
                    (int? v2))
               (if (> (int-num v1) (int-num v2))
                   (eval-under-env-c (ifgreater-e3 e) env)
                   (eval-under-env-c (ifgreater-e4 e) env))
               (error "MUPL addition applied to non-number")))]
        [(apair? e)
         (let ([v1 (eval-under-env-c (apair-e1 e) env)]
               [v2 (eval-under-env-c (apair-e2 e) env)])
           (apair v1 v2))]
        [(fst? e)
         (let ([v (eval-under-env-c (fst-e e) env)])
           (if (apair? v)
               (apair-e1 v)
               (error "MUPL fst applied to non-apair")))]
        [(snd? e)
         (let ([v (eval-under-env-c (snd-e e) env)])
           (if (apair? v)
               (apair-e2 v)
               (error "MUPL snd applied to non-apair")))]
        [(mlet? e)  
           (let* ([exp (eval-under-env-c (mlet-e e) env)]
                  [env2 (append (list (cons (mlet-var e) exp)) env)])
             (eval-under-env-c (mlet-body e) env2))]
        [(fun-challenge? e) (closure
                             (filter (lambda (v) (set-member? (fun-challenge-freevars e) v)) env) e)]
        [(call? e) (let ([func (eval-under-env-c (call-funexp e) env)]
                         [args (eval-under-env-c (call-actual e) env)])
                     (if (closure? func)
                         (let* ([fun1 (closure-fun func)]
                                [envr (closure-env func)]
                                [name (fun-challenge-nameopt fun1)]
                                [arg (cons (cons (fun-challenge-formal fun1) args)
                                           envr)])
                           (eval-under-env-c (fun-challenge-body fun1) 
                                             (if (string? name)
                                                 (cons (cons name envr) arg) ;;strip off
                                                 (arg)))) ;;strip off
                         (error "MUPL call applied to non-closure")))]  
        [(isaunit? e)
         (if (aunit? (eval-under-env-c (isaunit-e e) env))
             (int 1)
             (int 0))] ;; evaluate to 1 if e is unit else 0
        [(int? e) e] ;; a constant number, e.g., (int 17)   
        [(aunit? e) e]
        [(closure? e) e]
        [#t (error (format "bad MUPL expression: ~v" e))]))

;; Do NOT change this
(define (eval-exp-c e)
  (eval-under-env-c (compute-free-vars e) null))
