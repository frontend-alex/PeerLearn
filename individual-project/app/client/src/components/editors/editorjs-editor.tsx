import { useEffect, useRef } from "react";

export type OutputData = {
  blocks: Array<{
    type: string;
    data: Record<string, any>;
  }>;
  time?: number;
  version?: string;
};

interface EditorJSEditorProps {
  data?: OutputData;
  onChange?: (data: OutputData) => void;
  placeholder?: string;
  documentKey?: string; 
}

export const EditorJSEditor = ({
  data,
  onChange,
  placeholder = "Start writing...",
  documentKey,
}: EditorJSEditorProps) => {
  const editorRef = useRef<any | null>(null);
  const holderRef = useRef<HTMLDivElement>(null);
  const isInitializedRef = useRef(false);

  const onChangeRef = useRef(onChange);

  useEffect(() => {
    onChangeRef.current = onChange;
  }, [onChange]);

  useEffect(() => {
    if (!holderRef.current) return;

    let isMounted = true;

    const initEditor = async () => {
      if (editorRef.current) {
        try {
          await editorRef.current.isReady;
          editorRef.current.destroy();
        } catch (e) {}
        editorRef.current = null;
      }

      if (!isMounted) return;

      isInitializedRef.current = false;

      try {
        // Dynamically import EditorJS and plugins to reduce initial bundle size
        const [
          { default: EditorJS },
          { default: Header },
          { default: List },
          { default: Paragraph },
          { default: Quote },
          { default: Code },
          { default: LinkTool },
          { default: Marker },
          { default: Underline },
        ] = await Promise.all([
          import("@editorjs/editorjs"),
          import("@editorjs/header"),
          import("@editorjs/list"),
          import("@editorjs/paragraph"),
          import("@editorjs/quote"),
          import("@editorjs/code"),
          // @ts-ignore - no types available
          import("@editorjs/link"),
          // @ts-ignore - no types available
          import("@editorjs/marker"),
          import("@editorjs/underline"),
          
        ]);

        const editor = new EditorJS({
          holder: holderRef.current!,
          placeholder,
          tools: {
            header: {
              class: Header as any,
              config: {
                placeholder: "Enter a header",
                levels: [1, 2, 3, 4, 5, 6],
                defaultLevel: 2,
              },
            },
            paragraph: {
              class: Paragraph as any,
              inlineToolbar: true,
            },
            list: {
              class: List as any,
              inlineToolbar: true,
              config: {
                defaultStyle: "unordered",
              },
            },
            quote: {
              class: Quote as any,
              inlineToolbar: true,
            },
            code: Code as any,
            linkTool: LinkTool as any,
            marker: Marker as any,
            underline: Underline as any,
          },
          data: data || {
            blocks: [],
          },
          onChange: async () => {
            if (
              editorRef.current &&
              onChangeRef.current &&
              isInitializedRef.current
            ) {
              try {
                const outputData = await editorRef.current.save();
                onChangeRef.current(outputData);
              } catch (error) {
                console.error("Error saving editor data:", error);
              }
            }
          },
        });

        await editor.isReady;

        if (!isMounted) {
          editor.destroy();
          return;
        }

        editorRef.current = editor;
        isInitializedRef.current = true;
      } catch (error) {
        console.error("Error initializing editor:", error);
      }
    };

    initEditor();

    return () => {
      isMounted = false;
      isInitializedRef.current = false;

      if (editorRef.current) {
        try {
          editorRef.current.destroy();
        } catch (e) {}
        editorRef.current = null;
      }
    };
  }, [documentKey]);
  return <div ref={holderRef} id="editorjs" />;
};
