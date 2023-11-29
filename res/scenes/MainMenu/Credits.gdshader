shader_type canvas_item;

const vec4 COL_ONE = vec4(0.06, 0.06, 0.06, 1.0);
const vec4 COL_TWO = vec4(0.129, 0.141, 0.157, 1.0);

void fragment() {
	// Place fragment code here.
	
	vec4 col = mix(COL_ONE, COL_TWO, UV.y);
	COLOR = col;
}
