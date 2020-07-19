(define (problem dobot01)
(:domain BLOCKS)
(:objects Sphere cube_red_0 cube_blue_0 cube_yellow_0 - block
posyellow posred posblue - position)
(:init (HANDEMPTY) (free posyellow)(free posred)(free posblue)(ontable cube_red_0)(ontable cube_blue_0)(ontable cube_yellow_0)(clear Sphere)(clear cube_red_0)(clear cube_blue_0)(clear cube_yellow_0))
(:goal (and (free posyellow)(free posred)(free posblue)(ontable cube_red_0)(ontable cube_blue_0)(ontable cube_yellow_0)(clear Sphere)(clear cube_red_0)(clear cube_blue_0)(clear cube_yellow_0)))
)