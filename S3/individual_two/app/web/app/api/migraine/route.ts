import { NextRequest, NextResponse } from "next/server";

import { migraineInputSchema, migraineOutputSchema } from "@/api/migraine";

const API_BASE_URL = process.env.FASTAPI_BASE_URL ?? "http://127.0.0.1:8000";

export async function POST(request: NextRequest) {
  const body = await request.json();
  const parsedBody = migraineInputSchema.safeParse(body);

  if (!parsedBody.success) {
    return NextResponse.json(
      { error: "Invalid migraine input", issues: parsedBody.error.issues },
      { status: 400 },
    );
  }

  const response = await fetch(`${API_BASE_URL}/predict`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(parsedBody.data),
  });

  if (!response.ok) {
    return NextResponse.json(
      { error: "Migraine prediction service failed" },
      { status: response.status },
    );
  }

  const parsedResponse = migraineOutputSchema.safeParse(await response.json());

  if (!parsedResponse.success) {
    return NextResponse.json(
      { error: "Invalid migraine prediction response" },
      { status: 502 },
    );
  }

  return NextResponse.json(parsedResponse.data);
}
