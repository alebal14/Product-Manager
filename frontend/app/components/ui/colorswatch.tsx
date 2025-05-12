import { ColorDisplay } from "@/app/interfaces";
import React from "react";

interface ColorSwatchProps {
  color: ColorDisplay;
  size?: "sm" | "md" | "lg";
}

const sizeClasses = {
  sm: "w-6 h-6",
  md: "w-8 h-8",
  lg: "w-10 h-10",
};

export function ColorSwatch({ color, size = "md" }: ColorSwatchProps) {
  return (
    <div className="group relative">
      <div
        className={`${sizeClasses[size]} rounded-full border-2 border-gray-300 cursor-pointer hover:scale-110 transition-transform`}
        style={{ backgroundColor: color.hex }}
        title={color.name}
      />
      <span className="absolute bottom-full left-1/2 transform -translate-x-1/2 mb-1 px-2 py-1 text-xs text-white bg-gray-800 rounded opacity-0 group-hover:opacity-100 transition-opacity whitespace-nowrap">
        {color.name}
      </span>
    </div>
  );
}
