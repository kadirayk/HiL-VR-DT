(define (domain BLOCKS)
  (:requirements :strips :typing)
  (:types block position)
  (:predicates (on ?x - block ?y - block)
           (at ?x - block ?p - position)
           (free ?p - position)
	       (ontable ?x - block)
	       (onload ?x - block)
	       (onDetection2 ?x - block)
	       (clear ?x - block)
	       (handempty)
	       (loadfree)
	       (holding ?x - block)
	       (done)
	       )

  (:action place
	     :parameters (?x - block ?p - position)
	     :precondition (and (holding ?x) (free ?p))
	     :effect
	     (and (not (holding ?x))
		   (not (free ?p))
		   (clear ?x)
		   (handempty)
		   (at ?x ?p) (done)))
		   
  (:action load
         :parameters (?x - block)
         :precondition (and (clear ?x) (ontable ?x) (loadfree) (done))
         :effect
         (and (not (ontable ?x))
           (not (loadfree))
           (onload ?x) (not (done))))
           
  (:action moveonconveyor
         :parameters (?x - block)
         :precondition (and (onload ?x))
         :effect
         (and (loadfree)
           (onDetection2 ?x)))
	     
  (:action pick-up
	     :parameters (?x - block)
	     :precondition (and (clear ?x) (onDetection2 ?x) (handempty))
	     :effect
	     (and (not (onDetection2 ?x))
		   (not (clear ?x))
		   (not (handempty))
		   (holding ?x)))
		   
  (:action pick-from-pos
	     :parameters (?x - block ?p - position)
	     :precondition (and (clear ?x) (at ?x ?p) (handempty))
	     :effect
	     (and (not (at ?x ?p))
		   (not (clear ?x))
		   (not (handempty))
		   (free ?p)
		   (holding ?x)))

  (:action put-down
	     :parameters (?x - block)
	     :precondition (holding ?x)
	     :effect
	     (and (not (holding ?x))
		   (clear ?x)
		   (handempty)
		   (ontable ?x)))
  (:action stack
	     :parameters (?x - block ?y - block)
	     :precondition (and (holding ?x) (clear ?y))
	     :effect
	     (and (not (holding ?x))
		   (not (clear ?y))
		   (clear ?x)
		   (handempty)
		   (on ?x ?y)
		   (done)))
  (:action unstack
	     :parameters (?x - block ?y - block)
	     :precondition (and (on ?x ?y) (clear ?x) (handempty))
	     :effect
	     (and (holding ?x)
		   (clear ?y)
		   (not (clear ?x))
		   (not (handempty))
		   (not (on ?x ?y))
		   (not (done)))))