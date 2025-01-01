shader_type canvas_item;

uniform vec4 replace_color : source_color = vec4(1.0, 0.0, 0.0, 1.0); // Varsayılan kırmızı
uniform float threshold : hint_range(0.0, 1.0) = 0.1;

void fragment() {
    vec4 current_color = texture(TEXTURE, UV);
    
    // Sadece siyah pikselleri değiştir (RGB değerleri threshold'dan küçük)
    if (length(current_color.rgb) < threshold) {
        COLOR = vec4(replace_color.rgb, current_color.a);
    } else {
        COLOR = current_color;
    }
}