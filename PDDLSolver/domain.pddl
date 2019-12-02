(define (domain CUBES)
  (:requirements :strips :typing)
  (:types cube position)
  (:predicates (at ?c - cube ?p - position)
           (free ?p - position))

  (:action move
	     :parameters (?c - cube ?from ?to - position)
	     :precondition (and (free ?to) (at ?c ?from))
	     :effect (and (not (at ?c ?from)) (at ?c ?to) (not (free ?to)) (free ?from)))
)