/**
 * Node Floating Toolbar Component
 * 
 * A floating toolbar that appears above a selected node, providing quick access
 * to node properties like color. Optimized for performance with reduced update frequency.
 * 
 * @module whiteboard/components/NodeFloatingToolbar
 */

import { useEffect, useState, useCallback, useMemo, useRef } from "react";
import { useReactFlow, type Node, useStore } from "@xyflow/react";
import { randomColorsDark } from "@/components/ui/consts/consts";
import type {
  RectangleNodeData,
  CircleNodeData,
  DiamondNodeData,
} from "./nodes";
import { WHITEBOARD_CONFIG } from "../config/whiteboard-config";

/**
 * Props for NodeFloatingToolbar component
 */
interface NodeFloatingToolbarProps {
  /** Currently selected nodes (only first node is used) */
  selectedNodes: Node[];
  /** Callback to update node data */
  onUpdateNode: (nodeId: string, data: any) => void;
}

/**
 * Color palette for the toolbar (memoized to avoid recreation)
 */
const COLOR_PALETTE = randomColorsDark.slice(0, WHITEBOARD_CONFIG.toolbar.colorPalette.count);

/**
 * Update interval for toolbar position (milliseconds)
 * Reduced from 50ms to 100ms for better performance
 */
const POSITION_UPDATE_INTERVAL = 100;

/**
 * Node Floating Toolbar Component
 * 
 * Displays a floating toolbar above a selected node with color swatches.
 * Only shows when exactly one node is selected.
 * 
 * Performance optimizations:
 * - Uses requestAnimationFrame for position updates when dragging
 * - Memoizes color palette
 * - Debounces position updates
 * - Only updates position when node or viewport changes
 * 
 * @param props - Component props
 * @returns Floating toolbar component or null
 * 
 * @example
 * ```tsx
 * <NodeFloatingToolbar
 *   selectedNodes={selectedNodes}
 *   onUpdateNode={handleUpdateNode}
 * />
 * ```
 */
export function NodeFloatingToolbar({
  selectedNodes,
  onUpdateNode,
}: NodeFloatingToolbarProps) {
  const { getNode, flowToScreenPosition } = useReactFlow();
  const [position, setPosition] = useState({ x: 0, y: 0 });
  const intervalRef = useRef<NodeJS.Timeout | null>(null);
  const rafRef = useRef<number | null>(null);

  // Subscribe to viewport transform and node positions for reactive updates
  // Using useStore for fine-grained subscriptions to avoid unnecessary re-renders
  const transform = useStore((store) => store.transform);
  const nodes = useStore((store) => store.nodeLookup);

  /**
   * Update toolbar position based on selected node's DOM position
   */
  const updatePosition = useCallback(() => {
    if (selectedNodes.length !== 1) return;

    const node = selectedNodes[0];
    
    // Try to find the actual DOM element for this node
    // React Flow wraps nodes in .react-flow__node with data-id attribute
    const nodeDomElement = document.querySelector(
      `.react-flow__node[data-id="${node.id}"]`
    ) as HTMLElement;
    
    if (nodeDomElement) {
      // Use getBoundingClientRect for accurate screen position
      const rect = nodeDomElement.getBoundingClientRect();
      setPosition({
        x: rect.left + rect.width / 2,
        y: rect.top + WHITEBOARD_CONFIG.toolbar.position.offsetY,
      });
    } else {
      // Fallback: use flow coordinates if DOM element not found
      const nodeElement = getNode(node.id);
      if (nodeElement) {
        const nodeData = nodeElement.data as
          | RectangleNodeData
          | CircleNodeData
          | DiamondNodeData;
        const nodeWidth =
          (nodeData as RectangleNodeData).width ||
          (nodeData as CircleNodeData | DiamondNodeData).size ||
          150;

        const screenPos = flowToScreenPosition({
          x: nodeElement.position.x,
          y: nodeElement.position.y,
        });

        const reactFlowContainer = document.querySelector(".react-flow");
        if (reactFlowContainer) {
          const containerRect = reactFlowContainer.getBoundingClientRect();
          setPosition({
            x: containerRect.left + screenPos.x + nodeWidth / 2,
            y: containerRect.top + screenPos.y - 50,
          });
        }
      }
    }
  }, [selectedNodes, getNode, flowToScreenPosition]);

  // Update position when node or viewport changes
  useEffect(() => {
    if (selectedNodes.length === 1) {
      // Initial position update
      updatePosition();

      // Use requestAnimationFrame for smoother updates during interactions
      // Fall back to interval for viewport changes
      const scheduleUpdate = () => {
        if (rafRef.current) {
          cancelAnimationFrame(rafRef.current);
        }
        
        rafRef.current = requestAnimationFrame(() => {
          updatePosition();
        });
      };

      // Update on viewport or node changes with reduced frequency
      intervalRef.current = setInterval(scheduleUpdate, POSITION_UPDATE_INTERVAL);

      return () => {
        if (intervalRef.current) {
          clearInterval(intervalRef.current);
        }
        if (rafRef.current) {
          cancelAnimationFrame(rafRef.current);
        }
      };
    }
  }, [selectedNodes, nodes, transform, updatePosition]);

  // Don't render if no selection or multiple selections
  if (selectedNodes.length === 0 || selectedNodes.length > 1) {
    return null;
  }

  /**
   * Handle color change for selected node
   * 
   * @param color - New color value (hex string)
   */
  const handleColorChange = useCallback(
    (color: string) => {
      selectedNodes.forEach((node) => {
        const currentData = node.data as
          | RectangleNodeData
          | CircleNodeData
          | DiamondNodeData;
        onUpdateNode(node.id, {
          ...currentData,
          color,
        });
      });
    },
    [selectedNodes, onUpdateNode]
  );

  // Get current node and its data
  const node = selectedNodes[0];
  const nodeData = node.data as
    | RectangleNodeData
    | CircleNodeData
    | DiamondNodeData;
  const currentColor = nodeData.color || "#ffffff";

  // Memoize fill colors array to avoid recreation on every render
  const fillColors = useMemo(
    () => [...COLOR_PALETTE, "#ffffff"],
    []
  );

  return (
    <div
      className="fixed z-50 bg-background/95 backdrop-blur-sm border rounded-md shadow-lg p-1.5 flex items-center justify-center pointer-events-auto"
      style={{
        left: `${position.x}px`,
        top: `${position.y}px`,
        transform: "translate(-50%, -100%)",
        marginTop: "-8px",
      }}
      onMouseDown={(e) => e.stopPropagation()}
      onClick={(e) => e.stopPropagation()}
    >
      {/* Fill Colors - centered row */}
      <div className="flex items-center gap-1">
        {fillColors.map((color) => (
          <button
            key={`fill-${color}`}
            onClick={() => handleColorChange(color)}
            className={`w-5 h-5 rounded border-2 transition-all hover:scale-125 cursor-pointer ${
              currentColor === color
                ? "border-foreground ring-1 ring-offset-1 ring-foreground/50"
                : "border-border/50 hover:border-border"
            }`}
            style={{ backgroundColor: color }}
            title={color}
            aria-label={`Change color to ${color}`}
          />
        ))}
      </div>
    </div>
  );
}
