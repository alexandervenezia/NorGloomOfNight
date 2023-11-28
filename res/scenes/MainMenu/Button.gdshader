shader_type canvas_item;

uniform float isHighlighted;
const vec4 BRIGHT_RIM = vec4(0.9, 0.9, 1.0, 1);
const vec4 INTERIOR = vec4(0.6, 0.7, 0.8, 1);

varying vec2 world_pos;

void vertex()
{
	world_pos = (MODEL_MATRIX * vec4(VERTEX, 1.0, 1.0)).xy;
}

void fragment() {
	vec4 col = vec4(world_pos, 1.0, 1.0);
	
	
	
	COLOR = col;
}
