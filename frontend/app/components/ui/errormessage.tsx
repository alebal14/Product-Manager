import React from "react";
import Link from "next/link";

interface ErrorDisplayProps {
  message: string;
}

export function ErrorMessage({ message }: ErrorDisplayProps) {
  return (
    <div className="min-h-screen flex items-center justify-center">
      <div className="text-center">
        <div className="text-xl text-red-600 mb-4">{message}</div>
      </div>
    </div>
  );
}
