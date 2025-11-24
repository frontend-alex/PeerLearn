import { Separator } from "@/components/ui/separator";

const WorkspaceInfo = () => {
  return (
    <div className="space-y-8">
      <div>
        <h1 className="text-2xl font-bold">Update your Workspace</h1>
        <p className="text-muted-foreground text-sm text-balance">
          Enter your email below to recover to your account information
        </p>
      </div>

    <Separator className="bg-accent"/>

      <div className="grid grid-cols-3 gap-8 items-start"></div>
    </div>
  );
};

export default WorkspaceInfo;
