
shader_type canvas_item;

uniform float isHighlighted;
uniform float width;
uniform float height;

const vec4 BRIGHT_RIM = vec4(0.9, 0.9, 1.0, 1);
const vec4 INTERIOR = vec4(0.6, 0.7, 0.8, 1);



void fragment() {
	float aspect = width/height;
	
	float radius = 0.25;

	vec4 col = vec4(0.0, 0.0, 0.0, 1.0); //INTERIOR;
	//col.r = step(distance(UV.x, 0.5), ((1.0) - radius * (height/width))/2.0);
	//col.r = smoothstep(1.0, 0.0, step(distance(UV.x, 0.5), ((1.0) - radius * (height/width))/2.0));
	
	vec2 adjustedUV = abs(UV - vec2(0.5, 0.5));
	float dist = length(adjustedUV);
	
	
	//if (adjustedUV.x > 0.5 - radius/aspect && adjustedUV.y > 0.5 - radius)
		//col.a = 0.0;
		
	//if (dist > 1.0 - radius)
		//col.a = 0.0;
	
	//col.r = step(distance(adjustedUV, vec2(0.0, 0.0)), ((1.0) - radius)/2.0);
	//col.r = distance(adjustedUV, vec2(0.0, 0.0)) * 2.0;
	//col.r = adjustedUV.x;
	//col.g = step(distance(UV.y, 0.5), ((1.0) - radius)/2.0);
	
	
	
	COLOR = col;
}


