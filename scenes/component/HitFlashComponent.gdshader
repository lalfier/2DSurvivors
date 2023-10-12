shader_type canvas_item;

uniform float lerpPercent : hint_range(0.0, 1.0);

void fragment() {
	// Called for every pixel the material is visible on.
	vec4 textureColor = texture(TEXTURE, UV);
	vec4 finalColor = mix(textureColor, vec4(1.0, 1.0, 1.0, textureColor.a), lerpPercent);
	COLOR = finalColor;
}