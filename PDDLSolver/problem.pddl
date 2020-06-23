(define (problem dobot01)
(:domain BLOCKS)
(:objects cube_yellow_0 cube_red_0 cube_blue_0 - block
posred posblue posyellow - position)
(:init (HANDEMPTY) (free posred)(free posblue)(free posyellow)(ontable cube_yellow_0)(ontable cube_red_0)(ontable cube_blue_0)(clear cube_yellow_0)(clear cube_red_0)(clear cube_blue_0))
(:goal (and (at cube_red_0 posred)(at cube_blue_0 posblue)(at cube_yellow_0 posyellow)(clear cube_yellow_0)(clear cube_red_0)(clear cube_blue_0)))
)