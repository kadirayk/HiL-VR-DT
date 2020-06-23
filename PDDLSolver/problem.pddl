(define (problem dobot01)
(:domain BLOCKS)
(:objects cube_0 cube_1 - block
pos0 pos1 - position)
(:init (done)(HANDEMPTY) (loadfree) (free pos0)(free pos1)
(ontable cube_0)(ontable cube_1)
(clear cube_0)(clear cube_1))
(:goal (and (at cube_0 pos0) (at cube_1 pos1)))
)