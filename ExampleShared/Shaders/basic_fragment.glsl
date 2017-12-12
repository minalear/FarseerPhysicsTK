#version 400
out vec4 fragmentColor;

uniform vec4 drawColor;

void main() {
    fragmentColor = drawColor;
}