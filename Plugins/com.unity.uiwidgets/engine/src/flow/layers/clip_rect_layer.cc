#include "flow/layers/clip_rect_layer.h"

namespace uiwidgets {

ClipRectLayer::ClipRectLayer(const SkRect& clip_rect, Clip clip_behavior)
    : clip_rect_(clip_rect), clip_behavior_(clip_behavior) {
  FML_DCHECK(clip_behavior != Clip::none);
}

void ClipRectLayer::Preroll(PrerollContext* context, const SkMatrix& matrix) {
  TRACE_EVENT0("uiwidgets", "ClipRectLayer::Preroll");

  SkRect previous_cull_rect = context->cull_rect;
  children_inside_clip_ = context->cull_rect.intersect(clip_rect_);
  if (children_inside_clip_) {
    TRACE_EVENT_INSTANT0("uiwidgets", "children inside clip rect");

    Layer::AutoPrerollSaveLayerState save =
        Layer::AutoPrerollSaveLayerState::Create(context, UsesSaveLayer());
    context->mutators_stack.PushClipRect(clip_rect_);
    SkRect child_paint_bounds = SkRect::MakeEmpty();
    PrerollChildren(context, matrix, &child_paint_bounds);

    if (child_paint_bounds.intersect(clip_rect_)) {
      set_paint_bounds(child_paint_bounds);
    }
    context->mutators_stack.Pop();
  }
  context->cull_rect = previous_cull_rect;
}

void ClipRectLayer::Paint(PaintContext& context) const {
  TRACE_EVENT0("uiwidgets", "ClipRectLayer::Paint");
  FML_DCHECK(needs_painting());

  if (!children_inside_clip_) {
    TRACE_EVENT_INSTANT0("uiwidgets",
                         "children not inside clip rect, skipping");
    return;
  }

  SkAutoCanvasRestore save(context.internal_nodes_canvas, true);
  context.internal_nodes_canvas->clipRect(clip_rect_,
                                          clip_behavior_ != Clip::hardEdge);

  if (UsesSaveLayer()) {
    context.internal_nodes_canvas->saveLayer(clip_rect_, nullptr);
  }
  PaintChildren(context);
  if (UsesSaveLayer()) {
    context.internal_nodes_canvas->restore();
  }
}

}  // namespace uiwidgets
