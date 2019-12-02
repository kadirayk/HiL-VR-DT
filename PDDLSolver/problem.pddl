(define (problem dobot01)
(:domain CUBES)
(:objects Cube1 Cube1 - cube
posA posB posC - position)
(:init (at Cube1 posA)(free posB)(free posC))
(:goal (and (at Cube1 posC)))
)