(define (problem dobot01)
(:domain CUBES)
(:objects BlueCube - cube
posred posblue - position)
(:init (at BlueCube posred)(free posblue))
(:goal (and (at BlueCube posblue)))
)