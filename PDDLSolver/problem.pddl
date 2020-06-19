(define (problem dobot01)
(:domain BLOCKS)
(:objects cube_0 cube_1 cube_2 - block
posred posblue posyellow - position)
(:init (HANDEMPTY) (free posred)(free posblue)(free posyellow)(ontable cube_0)(ontable cube_1)(ontable cube_2)(clear cube_0)(clear cube_1)(clear cube_2))
(:goal (and (at cube_1 posblue)(free posred)(free posyellow)(on cube_0 cube_2)(on cube_2 cube_1)(clear cube_0)))
)