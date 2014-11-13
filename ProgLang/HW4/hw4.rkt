
#lang racket

(provide (all-defined-out)) ;; so we can put tests in a second file

;; put your code below

(define (sequence low high stride)
  (if (<= low high)
      (append (cons low null) (sequence (+ low stride) high stride))
      null))

;(define (sequence2 low high stride)
;  (letrec ([f (lambda (x ans)
;                (if (<= low x high)
;                    (f (+ x stride) (append ans (cons x null)))
;                    ans))])
;    (f low null)))

(define (string-append-map xs suffix)
  (map (lambda (x) (string-append x suffix)) xs))

(define (list-nth-mod xs n)
  (let ([len (length xs)])
        (cond [(< n 0) (error "list-nth-mod: negative number")]
              [(<= len 0) (error "list-nth-mod: empty list")]
              [#t (car (list-tail xs (remainder n len)))])))

(define (stream-for-n-steps s n)
      (cond [(= n 0) null]
            [#t (cons (car (s)) (stream-for-n-steps (cdr (s)) (- n 1)))]))

(define funny-number-stream 
  (letrec ([f (lambda (x)
                (if (= (remainder x 5) 0)
                    (cons (- 0 x) (lambda () (f (+ x 1))))
                    (cons x (lambda () (f (+ x 1))))))])
    (lambda () (f 1))))

(define dan-then-dog 
  (letrec ([f (lambda (x)
                (if (= x 0)
                    (cons "dan.jpg" (lambda () (f 1)))
                    (cons "dog.jpg" (lambda () (f 0)))))])
    (lambda () (f 0))))
                
(define (stream-add-zero s)
       (letrec ([f (lambda(stream)
                (lambda() (cons (cons 0 (car (stream))) (f (cdr (stream))))))])
      (f s)))
                  
(define (cycle-lists xs ys)
  (letrec (
           [f (lambda (n)
                (let ([x (list-nth-mod xs n)]
                      [y (list-nth-mod ys n)])
                  (cons (cons x y) (lambda () (f (+ n 1))))))])
    (lambda () (f 0))))

(define (vector-assoc v vec)
  (letrec ([len (vector-length vec)]
           [f (lambda(n)
                (cond [(= n len) #f]
                      [(and (pair? (vector-ref vec n)) (equal? (car (vector-ref vec n)) v)) (vector-ref vec n)]
                      [#t (f (+ n 1))]))])
    (f 0)))

(define (cached-assoc xs n)
  (letrec([memo (make-vector n)]
          [index 0]
          [f (lambda(v)
               (cond [(vector-assoc v memo)]
                     [(let ([result (assoc v xs)])
                        (cond [result (begin 
                                        (vector-set! memo index result)
                                        (set! index (remainder (+ index 1) n))
                                        result)]
                              [#t #f]))]))
             ])
    f))

(define-syntax while-less
  (syntax-rules (do)
    [(while-less e1 do e2)
     (let ([t e1])
       (letrec ([loop (lambda (r)
                        (if (>= r t)
                            #t
                            (loop e2)))])
         (loop e2)))]))
