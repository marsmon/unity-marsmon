#include "flow/layers/container_layer.h"

namespace uiwidgets {

class ClipPathLayer : public ContainerLayer {
 public:
  ClipPathLayer(const SkPath& clip_path, Clip clip_behavior = Clip::antiAlias);

  void Preroll(PrerollContext* context, const SkMatrix& matrix) override;

  void Paint(PaintContext& context) const override;

  bool UsesSaveLayer() const {
    return clip_behavior_ == Clip::antiAliasWithSaveLayer;
  }

 private:
  SkPath clip_path_;
  Clip clip_behavior_;
  bool children_inside_clip_ = false;

  FML_DISALLOW_COPY_AND_ASSIGN(ClipPathLayer);
};

}  // namespace uiwidgets
