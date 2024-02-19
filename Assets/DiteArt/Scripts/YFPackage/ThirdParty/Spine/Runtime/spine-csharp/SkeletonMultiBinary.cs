/******************************************************************************
 * Spine Runtimes License Agreement
 * Last updated January 1, 2020. Replaces all prior versions.
 *
 * Copyright (c) 2013-2020, Esoteric Software LLC
 *
 * Integration of the Spine Runtimes into software or otherwise creating
 * derivative works of the Spine Runtimes is permitted under the terms and
 * conditions of Section 2 of the Spine Editor License Agreement:
 * http://esotericsoftware.com/spine-editor-license
 *
 * Otherwise, it is permitted to integrate the Spine Runtimes into software
 * or otherwise create derivative works of the Spine Runtimes (collectively,
 * "Products"), provided that each user of the Products must obtain their own
 * Spine Editor license and redistribution of the Products in any form must
 * include this license and copyright notice.
 *
 * THE SPINE RUNTIMES ARE PROVIDED BY ESOTERIC SOFTWARE LLC "AS IS" AND ANY
 * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL ESOTERIC SOFTWARE LLC BE LIABLE FOR ANY
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES,
 * BUSINESS INTERRUPTION, OR LOSS OF USE, DATA, OR PROFITS) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
 * THE SPINE RUNTIMES, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 *****************************************************************************/

#if (UNITY_5 || UNITY_5_3_OR_NEWER || UNITY_WSA || UNITY_WP8 || UNITY_WP8_1)
#define IS_UNITY
#endif

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;

#if WINDOWS_STOREAPP
using System.Threading.Tasks;
using Windows.Storage;
#endif

namespace Spine {
	public class SkeletonMultiBinary : SkeletonMultiLoader {
		public const int BONE_ROTATE = 0;
		public const int BONE_TRANSLATE = 1;
		public const int BONE_TRANSLATEX = 2;
		public const int BONE_TRANSLATEY = 3;
		public const int BONE_SCALE = 4;
		public const int BONE_SCALEX = 5;
		public const int BONE_SCALEY = 6;
		public const int BONE_SHEAR = 7;
		public const int BONE_SHEARX = 8;
		public const int BONE_SHEARY = 9;

		public const int SLOT_ATTACHMENT = 0;
		public const int SLOT_RGBA = 1;
		public const int SLOT_RGB = 2;
		public const int SLOT_RGBA2 = 3;
		public const int SLOT_RGB2 = 4;
		public const int SLOT_ALPHA = 5;

		public const int PATH_POSITION = 0;
		public const int PATH_SPACING = 1;
		public const int PATH_MIX = 2;

		public const int CURVE_LINEAR = 0;
		public const int CURVE_STEPPED = 1;
		public const int CURVE_BEZIER = 2;

		public SkeletonMultiBinary (AttachmentLoader attachmentLoader)
			: base(attachmentLoader) {
		}

		public SkeletonMultiBinary (params Atlas[] atlasArray)
			: base(atlasArray) {
		}

#if !ISUNITY && WINDOWS_STOREAPP
		private async Task<SkeletonData> ReadFile(string path) {
			var folder = Windows.ApplicationModel.Package.Current.InstalledLocation;
			using (var input = new BufferedStream(await folder.GetFileAsync(path).AsTask().ConfigureAwait(false))) {
				SkeletonData skeletonData = ReadSkeletonData(input);
				skeletonData.Name = Path.GetFileNameWithoutExtension(path);
				return skeletonData;
			}
		}

		public override SkeletonData ReadSkeletonData (string path) {
			return this.ReadFile(path).Result;
		}
#else
		public override SkeletonData ReadSkeletonData (string path, string path2, string path3) {
#if WINDOWS_PHONE
			var input = new BufferedStream(Microsoft.Xna.Framework.TitleContainer.OpenStream(path))
#else
			var input = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
			var input2 = new FileStream(path2, FileMode.Open, FileAccess.Read, FileShare.Read);
			var input3 = new FileStream(path3, FileMode.Open, FileAccess.Read, FileShare.Read);
#endif
            SkeletonData skeletonData = ReadSkeletonData(input, input2, input3);
            skeletonData.name = Path.GetFileNameWithoutExtension(path);
            return skeletonData;
			
		}
#endif // WINDOWS_STOREAPP

		public static readonly TransformMode[] TransformModeValues = {
			TransformMode.Normal,
			TransformMode.OnlyTranslation,
			TransformMode.NoRotationOrReflection,
			TransformMode.NoScale,
			TransformMode.NoScaleOrReflection
		};

		/// <summary>Returns the version string of binary skeleton data.</summary>
		public static string GetVersionString (Stream file) {
			if (file == null) throw new ArgumentNullException("file");

			SkeletonInput input = new SkeletonInput(file);
			return input.GetVersionString();
		}

		public SkeletonData ReadSkeletonData (Stream file, Stream file2, Stream file3) {
			if (file == null || file2 == null || file3 == null) throw new ArgumentNullException("file");
			float scale = this.scale;

			var skeletonData = new SkeletonData();
			SkeletonInput input = new SkeletonInput(file);
			SkeletonInput input2 = new SkeletonInput(file2);
			SkeletonInput input3 = new SkeletonInput(file3);

			long hash = input.ReadLong();
			skeletonData.hash = hash == 0 ? null : hash.ToString();
			skeletonData.version = input.ReadString();
			if (skeletonData.version.Length == 0) skeletonData.version = null;
			// early return for old 3.8 format instead of reading past the end
			if (skeletonData.version.Length > 13) return null;
			skeletonData.x = input.ReadFloat();
			skeletonData.y = input.ReadFloat();
			skeletonData.width = input.ReadFloat();
			skeletonData.height = input.ReadFloat();

			bool nonessential = input.ReadBoolean();

			if (nonessential) {
				skeletonData.fps = input.ReadFloat();

				skeletonData.imagesPath = input.ReadString();
				if (string.IsNullOrEmpty(skeletonData.imagesPath)) skeletonData.imagesPath = null;

				skeletonData.audioPath = input.ReadString();
				if (string.IsNullOrEmpty(skeletonData.audioPath)) skeletonData.audioPath = null;
			}

            // 忽略后面2个文件的头部信息
            input2.ReadLong();
            input2.ReadString();
            input2.ReadFloat();
            input2.ReadFloat();
            input2.ReadFloat();
            input2.ReadFloat();
            bool nonessential2 = input2.ReadBoolean();
            if (nonessential2) {
                input2.ReadFloat();
                input2.ReadString();
                input2.ReadString();
            }

            input3.ReadLong();
            input3.ReadString();
            input3.ReadFloat();
            input3.ReadFloat();
            input3.ReadFloat();
            input3.ReadFloat();
            bool nonessential3 = input3.ReadBoolean();
            if (nonessential3) {
                input3.ReadFloat();
                input3.ReadString();
                input3.ReadString();
            }

			int n;
			int n2;
			int n3;
			Object[] o;
			Object[] o2;
			Object[] o3;

			// Strings.
			o = input.strings = new String[n = input.ReadInt(true)];
			for (int i = 0; i < n; i++)
				o[i] = input.ReadString();

            o2 = input2.strings = new String[n = input2.ReadInt(true)];
			for (int i = 0; i < n; i++)
				o2[i] = input2.ReadString();

            o3 = input3.strings = new String[n = input3.ReadInt(true)];
			for (int i = 0; i < n; i++)
				o3[i] = input3.ReadString();

			// Bones.
            n = input.ReadInt(true);
            n2 = input2.ReadInt(true);
            n3 = input3.ReadInt(true);

            skeletonData.boneOffset2 = n - 1;
            skeletonData.boneOffset3 = n + n2 - 2; // 第二第三个去除了root,所以需要偏移

			var bones = skeletonData.bones.Resize(n + n2 + n3 - 2).Items;
			for (int i = 0; i < n; i++) {
				String name = input.ReadString();
				BoneData parent = i == 0 ? null : bones[input.ReadInt(true)];
				BoneData data = new BoneData(i, name, parent);
				data.rotation = input.ReadFloat();
				data.x = input.ReadFloat() * scale;
				data.y = input.ReadFloat() * scale;
				data.scaleX = input.ReadFloat();
				data.scaleY = input.ReadFloat();
				data.shearX = input.ReadFloat();
				data.shearY = input.ReadFloat();
				data.Length = input.ReadFloat() * scale;
				data.transformMode = TransformModeValues[input.ReadInt(true)];
				data.skinRequired = input.ReadBoolean();
				if (nonessential) input.ReadInt(); // Skip bone color.
				bones[i] = data;
			}

			for (int i = 0; i < n2; i++) {
				String name = input2.ReadString();
                BoneData parent = null;
                if (i > 0) {
                    int parentBoneIndex = input2.ReadInt(true);
                    if (parentBoneIndex == 0) {
                        parent = bones[0];
                    } else {
                        parent = bones[skeletonData.boneOffset2 + parentBoneIndex];
                    }
                }
				BoneData data = new BoneData(skeletonData.boneOffset2 + i, name, parent);
				data.rotation = input2.ReadFloat();
				data.x = input2.ReadFloat() * scale;
				data.y = input2.ReadFloat() * scale;
				data.scaleX = input2.ReadFloat();
				data.scaleY = input2.ReadFloat();
				data.shearX = input2.ReadFloat();
				data.shearY = input2.ReadFloat();
				data.Length = input2.ReadFloat() * scale;
				data.transformMode = TransformModeValues[input2.ReadInt(true)];
				data.skinRequired = input2.ReadBoolean();
				if (nonessential2) input2.ReadInt(); // Skip bone color.
                if (name == "root") {
                    // 忽略root
                    continue;
                } else {
				    bones[skeletonData.boneOffset2 + i] = data;
                }
			}

			for (int i = 0; i < n3; i++) {
				String name = input3.ReadString();
                BoneData parent = null;
                if (i > 0) {
                    int parentBoneIndex = input3.ReadInt(true);
                    if (parentBoneIndex == 0) {
                        parent = bones[0];
                    } else {
                        parent = bones[skeletonData.boneOffset3 + parentBoneIndex];
                    }
                }
				BoneData data = new BoneData(skeletonData.boneOffset3 + i, name, parent);
				data.rotation = input3.ReadFloat();
				data.x = input3.ReadFloat() * scale;
				data.y = input3.ReadFloat() * scale;
				data.scaleX = input3.ReadFloat();
				data.scaleY = input3.ReadFloat();
				data.shearX = input3.ReadFloat();
				data.shearY = input3.ReadFloat();
				data.Length = input3.ReadFloat() * scale;
				data.transformMode = TransformModeValues[input3.ReadInt(true)];
				data.skinRequired = input3.ReadBoolean();
				if (nonessential3) input3.ReadInt(); // Skip bone color.
                if (name == "root") {
                    continue;
                } else {
				    bones[skeletonData.boneOffset3 + i] = data;
                }
			}

			// Slots.
            n = input.ReadInt(true);
            n2 = input2.ReadInt(true);
            n3 = input3.ReadInt(true);

            skeletonData.slotOffset2 = n;
            skeletonData.slotOffset3 = n + n2;

			var slots = skeletonData.slots.Resize(n + n2 + n3).Items;
			for (int i = 0; i < n; i++) {
				String slotName = input.ReadString();
				BoneData boneData = bones[input.ReadInt(true)];
				SlotData slotData = new SlotData(i, slotName, boneData);
				int color = input.ReadInt();
				slotData.r = ((color & 0xff000000) >> 24) / 255f;
				slotData.g = ((color & 0x00ff0000) >> 16) / 255f;
				slotData.b = ((color & 0x0000ff00) >> 8) / 255f;
				slotData.a = ((color & 0x000000ff)) / 255f;

				int darkColor = input.ReadInt(); // 0x00rrggbb
				if (darkColor != -1) {
					slotData.hasSecondColor = true;
					slotData.r2 = ((darkColor & 0x00ff0000) >> 16) / 255f;
					slotData.g2 = ((darkColor & 0x0000ff00) >> 8) / 255f;
					slotData.b2 = ((darkColor & 0x000000ff)) / 255f;
				}

				slotData.attachmentName = input.ReadStringRef();
				slotData.blendMode = (BlendMode)input.ReadInt(true);
				slots[i] = slotData;
			}

            for (int i = 0; i < n2; i++) {
				String slotName = input2.ReadString();
				BoneData boneData = bones[skeletonData.boneOffset2 + input2.ReadInt(true)];
				SlotData slotData = new SlotData(skeletonData.slotOffset2 + i, slotName, boneData);
				int color = input2.ReadInt();
				slotData.r = ((color & 0xff000000) >> 24) / 255f;
				slotData.g = ((color & 0x00ff0000) >> 16) / 255f;
				slotData.b = ((color & 0x0000ff00) >> 8) / 255f;
				slotData.a = ((color & 0x000000ff)) / 255f;

				int darkColor = input2.ReadInt(); // 0x00rrggbb
				if (darkColor != -1) {
					slotData.hasSecondColor = true;
					slotData.r2 = ((darkColor & 0x00ff0000) >> 16) / 255f;
					slotData.g2 = ((darkColor & 0x0000ff00) >> 8) / 255f;
					slotData.b2 = ((darkColor & 0x000000ff)) / 255f;
				}

				slotData.attachmentName = input2.ReadStringRef();
				slotData.blendMode = (BlendMode)input2.ReadInt(true);
				slots[skeletonData.slotOffset2 + i] = slotData;
			}

            for (int i = 0; i < n3; i++) {
				String slotName = input3.ReadString();
				BoneData boneData = bones[skeletonData.boneOffset3 + input3.ReadInt(true)];
				SlotData slotData = new SlotData(skeletonData.slotOffset3 + i, slotName, boneData);
				int color = input3.ReadInt();
				slotData.r = ((color & 0xff000000) >> 24) / 255f;
				slotData.g = ((color & 0x00ff0000) >> 16) / 255f;
				slotData.b = ((color & 0x0000ff00) >> 8) / 255f;
				slotData.a = ((color & 0x000000ff)) / 255f;

				int darkColor = input3.ReadInt(); // 0x00rrggbb
				if (darkColor != -1) {
					slotData.hasSecondColor = true;
					slotData.r2 = ((darkColor & 0x00ff0000) >> 16) / 255f;
					slotData.g2 = ((darkColor & 0x0000ff00) >> 8) / 255f;
					slotData.b2 = ((darkColor & 0x000000ff)) / 255f;
				}

				slotData.attachmentName = input3.ReadStringRef();
				slotData.blendMode = (BlendMode)input3.ReadInt(true);
				slots[skeletonData.slotOffset3 + i] = slotData;
			}

			// IK constraints.
            n = input.ReadInt(true);
            n2 = input2.ReadInt(true);
            n3 = input3.ReadInt(true);

            skeletonData.ikConstraintOffset2 = n;
            skeletonData.ikConstraintOffset3 = n + n2;

			o = skeletonData.ikConstraints.Resize(n + n2 + n3).Items;
			for (int i = 0, nn; i < n; i++) {
				IkConstraintData data = new IkConstraintData(input.ReadString());
				data.order = input.ReadInt(true);
				data.skinRequired = input.ReadBoolean();
				var constraintBones = data.bones.Resize(nn = input.ReadInt(true)).Items;
				for (int ii = 0; ii < nn; ii++)
					constraintBones[ii] = bones[input.ReadInt(true)];
				data.target = bones[input.ReadInt(true)];
				data.mix = input.ReadFloat();
				data.softness = input.ReadFloat() * scale;
				data.bendDirection = input.ReadSByte();
				data.compress = input.ReadBoolean();
				data.stretch = input.ReadBoolean();
				data.uniform = input.ReadBoolean();
				o[i] = data;
			}

			for (int i = 0, nn; i < n2; i++) {
				IkConstraintData data = new IkConstraintData(input2.ReadString());
				data.order = input2.ReadInt(true);
				data.skinRequired = input2.ReadBoolean();
				var constraintBones = data.bones.Resize(nn = input2.ReadInt(true)).Items;
				for (int ii = 0; ii < nn; ii++)
					constraintBones[ii] = bones[skeletonData.boneOffset2 + input2.ReadInt(true)];
				data.target = bones[skeletonData.boneOffset2 + input2.ReadInt(true)];
				data.mix = input2.ReadFloat();
				data.softness = input2.ReadFloat() * scale;
				data.bendDirection = input2.ReadSByte();
				data.compress = input2.ReadBoolean();
				data.stretch = input2.ReadBoolean();
				data.uniform = input2.ReadBoolean();
				o[n + i] = data;
			}

			for (int i = 0, nn; i < n3; i++) {
				IkConstraintData data = new IkConstraintData(input3.ReadString());
				data.order = input3.ReadInt(true);
				data.skinRequired = input3.ReadBoolean();
				var constraintBones = data.bones.Resize(nn = input3.ReadInt(true)).Items;
				for (int ii = 0; ii < nn; ii++)
					constraintBones[ii] = bones[skeletonData.boneOffset3 + input3.ReadInt(true)];
				data.target = bones[skeletonData.boneOffset3 + input3.ReadInt(true)];
				data.mix = input3.ReadFloat();
				data.softness = input3.ReadFloat() * scale;
				data.bendDirection = input3.ReadSByte();
				data.compress = input3.ReadBoolean();
				data.stretch = input3.ReadBoolean();
				data.uniform = input3.ReadBoolean();
				o[n + n2 + i] = data;
			}

			// Transform constraints.
            n = input.ReadInt(true);
            n2 = input2.ReadInt(true);
            n3 = input3.ReadInt(true);

            skeletonData.transformConstraintOffset2 = n;
            skeletonData.transformConstraintOffset3 = n + n2;

			o = skeletonData.transformConstraints.Resize(n + n2 + n3).Items;
			for (int i = 0, nn; i < n; i++) {
				TransformConstraintData data = new TransformConstraintData(input.ReadString());
				data.order = input.ReadInt(true);
				data.skinRequired = input.ReadBoolean();
				var constraintBones = data.bones.Resize(nn = input.ReadInt(true)).Items;
				for (int ii = 0; ii < nn; ii++)
					constraintBones[ii] = bones[input.ReadInt(true)];
				data.target = bones[input.ReadInt(true)];
				data.local = input.ReadBoolean();
				data.relative = input.ReadBoolean();
				data.offsetRotation = input.ReadFloat();
				data.offsetX = input.ReadFloat() * scale;
				data.offsetY = input.ReadFloat() * scale;
				data.offsetScaleX = input.ReadFloat();
				data.offsetScaleY = input.ReadFloat();
				data.offsetShearY = input.ReadFloat();
				data.mixRotate = input.ReadFloat();
				data.mixX = input.ReadFloat();
				data.mixY = input.ReadFloat();
				data.mixScaleX = input.ReadFloat();
				data.mixScaleY = input.ReadFloat();
				data.mixShearY = input.ReadFloat();
				o[i] = data;
			}

            for (int i = 0, nn; i < n2; i++) {
				TransformConstraintData data = new TransformConstraintData(input2.ReadString());
				data.order = input2.ReadInt(true);
				data.skinRequired = input2.ReadBoolean();
				var constraintBones = data.bones.Resize(nn = input2.ReadInt(true)).Items;
				for (int ii = 0; ii < nn; ii++)
					constraintBones[ii] = bones[skeletonData.boneOffset2 + input2.ReadInt(true)];
				data.target = bones[skeletonData.boneOffset2 + input2.ReadInt(true)];
				data.local = input2.ReadBoolean();
				data.relative = input2.ReadBoolean();
				data.offsetRotation = input2.ReadFloat();
				data.offsetX = input2.ReadFloat() * scale;
				data.offsetY = input2.ReadFloat() * scale;
				data.offsetScaleX = input2.ReadFloat();
				data.offsetScaleY = input2.ReadFloat();
				data.offsetShearY = input2.ReadFloat();
				data.mixRotate = input2.ReadFloat();
				data.mixX = input2.ReadFloat();
				data.mixY = input2.ReadFloat();
				data.mixScaleX = input2.ReadFloat();
				data.mixScaleY = input2.ReadFloat();
				data.mixShearY = input2.ReadFloat();
				o[n + i] = data;
			}

            for (int i = 0, nn; i < n3; i++) {
				TransformConstraintData data = new TransformConstraintData(input3.ReadString());
				data.order = input3.ReadInt(true);
				data.skinRequired = input3.ReadBoolean();
				var constraintBones = data.bones.Resize(nn = input3.ReadInt(true)).Items;
				for (int ii = 0; ii < nn; ii++)
					constraintBones[ii] = bones[skeletonData.boneOffset3 + input3.ReadInt(true)];
				data.target = bones[input3.ReadInt(true)];
				data.local = input3.ReadBoolean();
				data.relative = input3.ReadBoolean();
				data.offsetRotation = input3.ReadFloat();
				data.offsetX = input3.ReadFloat() * scale;
				data.offsetY = input3.ReadFloat() * scale;
				data.offsetScaleX = input3.ReadFloat();
				data.offsetScaleY = input3.ReadFloat();
				data.offsetShearY = input3.ReadFloat();
				data.mixRotate = input3.ReadFloat();
				data.mixX = input3.ReadFloat();
				data.mixY = input3.ReadFloat();
				data.mixScaleX = input3.ReadFloat();
				data.mixScaleY = input3.ReadFloat();
				data.mixShearY = input3.ReadFloat();
				o[n + n2 + i] = data;
			}

			// Path constraints
            n = input.ReadInt(true);
            n2 = input2.ReadInt(true);
            n3 = input3.ReadInt(true);

            skeletonData.pathConstraintOffset2 = n;
            skeletonData.pathConstraintOffset3 = n + n2;

			o = skeletonData.pathConstraints.Resize(n + n2 + n3).Items;
			for (int i = 0, nn; i < n; i++) {
				PathConstraintData data = new PathConstraintData(input.ReadString());
				data.order = input.ReadInt(true);
				data.skinRequired = input.ReadBoolean();
				Object[] constraintBones = data.bones.Resize(nn = input.ReadInt(true)).Items;
				for (int ii = 0; ii < nn; ii++)
					constraintBones[ii] = bones[input.ReadInt(true)];
				data.target = slots[input.ReadInt(true)];
				data.positionMode = (PositionMode)Enum.GetValues(typeof(PositionMode)).GetValue(input.ReadInt(true));
				data.spacingMode = (SpacingMode)Enum.GetValues(typeof(SpacingMode)).GetValue(input.ReadInt(true));
				data.rotateMode = (RotateMode)Enum.GetValues(typeof(RotateMode)).GetValue(input.ReadInt(true));
				data.offsetRotation = input.ReadFloat();
				data.position = input.ReadFloat();
				if (data.positionMode == PositionMode.Fixed) data.position *= scale;
				data.spacing = input.ReadFloat();
				if (data.spacingMode == SpacingMode.Length || data.spacingMode == SpacingMode.Fixed) data.spacing *= scale;
				data.mixRotate = input.ReadFloat();
				data.mixX = input.ReadFloat();
				data.mixY = input.ReadFloat();
				o[i] = data;
			}

            for (int i = 0, nn; i < n2; i++) {
				PathConstraintData data = new PathConstraintData(input2.ReadString());
				data.order = input2.ReadInt(true);
				data.skinRequired = input2.ReadBoolean();
				Object[] constraintBones = data.bones.Resize(nn = input2.ReadInt(true)).Items;
				for (int ii = 0; ii < nn; ii++)
					constraintBones[ii] = bones[skeletonData.boneOffset2 + input2.ReadInt(true)];
				data.target = slots[skeletonData.slotOffset2 + input2.ReadInt(true)];
				data.positionMode = (PositionMode)Enum.GetValues(typeof(PositionMode)).GetValue(input2.ReadInt(true));
				data.spacingMode = (SpacingMode)Enum.GetValues(typeof(SpacingMode)).GetValue(input2.ReadInt(true));
				data.rotateMode = (RotateMode)Enum.GetValues(typeof(RotateMode)).GetValue(input2.ReadInt(true));
				data.offsetRotation = input2.ReadFloat();
				data.position = input2.ReadFloat();
				if (data.positionMode == PositionMode.Fixed) data.position *= scale;
				data.spacing = input2.ReadFloat();
				if (data.spacingMode == SpacingMode.Length || data.spacingMode == SpacingMode.Fixed) data.spacing *= scale;
				data.mixRotate = input2.ReadFloat();
				data.mixX = input2.ReadFloat();
				data.mixY = input2.ReadFloat();
				o[n + i] = data;
			}

            for (int i = 0, nn; i < n3; i++) {
				PathConstraintData data = new PathConstraintData(input3.ReadString());
				data.order = input3.ReadInt(true);
				data.skinRequired = input3.ReadBoolean();
				Object[] constraintBones = data.bones.Resize(nn = input3.ReadInt(true)).Items;
				for (int ii = 0; ii < nn; ii++)
					constraintBones[ii] = bones[skeletonData.boneOffset3 + input3.ReadInt(true)];
				data.target = slots[skeletonData.slotOffset3 + input3.ReadInt(true)];
				data.positionMode = (PositionMode)Enum.GetValues(typeof(PositionMode)).GetValue(input3.ReadInt(true));
				data.spacingMode = (SpacingMode)Enum.GetValues(typeof(SpacingMode)).GetValue(input3.ReadInt(true));
				data.rotateMode = (RotateMode)Enum.GetValues(typeof(RotateMode)).GetValue(input3.ReadInt(true));
				data.offsetRotation = input3.ReadFloat();
				data.position = input3.ReadFloat();
				if (data.positionMode == PositionMode.Fixed) data.position *= scale;
				data.spacing = input3.ReadFloat();
				if (data.spacingMode == SpacingMode.Length || data.spacingMode == SpacingMode.Fixed) data.spacing *= scale;
				data.mixRotate = input3.ReadFloat();
				data.mixX = input3.ReadFloat();
				data.mixY = input3.ReadFloat();
				o[n + n2 + i] = data;
			}

			// Default skin.
			Skin defaultSkin = ReadSkin(input, skeletonData, true, nonessential, 1);
			if (defaultSkin != null) {
				skeletonData.defaultSkin = defaultSkin;
				skeletonData.skins.Add(defaultSkin);

                ReadSkin(input2, skeletonData, true, nonessential2, 2);
                
                ReadSkin(input3, skeletonData, true, nonessential3, 3);
			}

			// Skins.
			{
				int i = skeletonData.skins.Count;
				o = skeletonData.skins.Resize(n = i + input.ReadInt(true)).Items;
				for (; i < n; i++)
					o[i] = ReadSkin(input, skeletonData, false, nonessential, 1);

                n2 = input2.ReadInt(true);
                for (i = 0; i < n2; i++)
					ReadSkin(input2, skeletonData, false, nonessential2, 2);

                n3 = input3.ReadInt(true);
                for (i = 0; i < n3; i++)
					ReadSkin(input3, skeletonData, false, nonessential2, 3);
			}

            

			// Linked meshes.
			n = linkedMeshes.Count;
			for (int i = 0; i < n; i++) {
				LinkedMesh linkedMesh = linkedMeshes[i];
				Skin skin = linkedMesh.skin == null ? skeletonData.DefaultSkin : skeletonData.FindSkin(linkedMesh.skin);
				if (skin == null) throw new Exception("Skin not found: " + linkedMesh.skin);
				Attachment parent = skin.GetAttachment(linkedMesh.slotIndex, linkedMesh.parent);
				if (parent == null) throw new Exception("Parent mesh not found: " + linkedMesh.parent);
				linkedMesh.mesh.DeformAttachment = linkedMesh.inheritDeform ? (VertexAttachment)parent : linkedMesh.mesh;
				linkedMesh.mesh.ParentMesh = (MeshAttachment)parent;
				linkedMesh.mesh.UpdateUVs();
			}
			linkedMeshes.Clear();

			// Events.
            n = input.ReadInt(true);
            n2 = input2.ReadInt(true);
            n3 = input3.ReadInt(true);

            skeletonData.eventOffset2 = n;
            skeletonData.eventOffset3 = n + n2;

			o = skeletonData.events.Resize(n + n2 + n3).Items;
			for (int i = 0; i < n; i++) {
				EventData data = new EventData(input.ReadStringRef());
				data.Int = input.ReadInt(false);
				data.Float = input.ReadFloat();
				data.String = input.ReadString();
				data.AudioPath = input.ReadString();
				if (data.AudioPath != null) {
					data.Volume = input.ReadFloat();
					data.Balance = input.ReadFloat();
				}
				o[i] = data;
			}

            for (int i = 0; i < n2; i++) {
				EventData data = new EventData(input2.ReadStringRef());
				data.Int = input2.ReadInt(false);
				data.Float = input2.ReadFloat();
				data.String = input2.ReadString();
				data.AudioPath = input2.ReadString();
				if (data.AudioPath != null) {
					data.Volume = input2.ReadFloat();
					data.Balance = input2.ReadFloat();
				}
				o[n + i] = data;
			}

            for (int i = 0; i < n3; i++) {
				EventData data = new EventData(input3.ReadStringRef());
				data.Int = input3.ReadInt(false);
				data.Float = input3.ReadFloat();
				data.String = input3.ReadString();
				data.AudioPath = input3.ReadString();
				if (data.AudioPath != null) {
					data.Volume = input3.ReadFloat();
					data.Balance = input3.ReadFloat();
				}
				o[n + n2 + i] = data;
			}

			// Animations.
            n = input.ReadInt(true);
            n2 = input2.ReadInt(true);
            n3 = input3.ReadInt(true);

			if (n != n2 || n != n3) throw new Exception("Animations Count Not Same");

			o = skeletonData.animations.Resize(n).Items;
			for (int i = 0; i < n; i++) {
                String name = input.ReadString();
                String name2 = input2.ReadString();
                String name3 = input3.ReadString();

				if (!name.Equals(name2) || !name.Equals(name3)) throw new Exception("Animations Name Not Same i:" + i + " name:" + name + " name3:" + name3);

				o[i] = ReadAnimation(name, input, input2, input3, skeletonData);
			}

			return skeletonData;
		}

		/// <returns>May be null.</returns>
		private Skin ReadSkin (SkeletonInput input, SkeletonData skeletonData, bool defaultSkin, bool nonessential, int idx = 1) {
			Skin skin;
			int slotCount;
            int boneOffset;
            int slotOffset;
            int ikConstraintOffset;
            int transformConstraintOffset;
            int pathConstraintOffset;
            switch (idx) {
                case 3:
                    boneOffset = skeletonData.boneOffset3;
                    slotOffset = skeletonData.slotOffset3;
                    ikConstraintOffset = skeletonData.ikConstraintOffset3;
                    transformConstraintOffset = skeletonData.transformConstraintOffset3;
                    pathConstraintOffset = skeletonData.pathConstraintOffset3;
                    break;

                case 2:
                    boneOffset = skeletonData.boneOffset2;
                    slotOffset = skeletonData.slotOffset2;
                    ikConstraintOffset = skeletonData.ikConstraintOffset2;
                    transformConstraintOffset = skeletonData.transformConstraintOffset2;
                    pathConstraintOffset = skeletonData.pathConstraintOffset2;
                    break;

                default:
                    boneOffset = 0;
                    slotOffset = 0;
                    ikConstraintOffset = 0;
                    transformConstraintOffset = 0;
                    pathConstraintOffset = 0;
                    break;
            }

			if (defaultSkin) {
				slotCount = input.ReadInt(true);
				if (slotCount == 0) return null;

                skin = skeletonData.FindSkin("default");
                if (skin == null) {
                    skin = new Skin("default");
                }
			} else {
                String name = input.ReadStringRef();
                skin = skeletonData.FindSkin(name);
                if (skin == null) {
                    skin = new Skin(name);
                }
                int start = skin.bones.Count;
				Object[] bones = skin.bones.Resize(start + input.ReadInt(true)).Items;
				var bonesItems = skeletonData.bones.Items;
				for (int i = start, n = skin.bones.Count; i < n; i++)
					bones[i] = bonesItems[boneOffset + input.ReadInt(true)];

				var ikConstraintsItems = skeletonData.ikConstraints.Items;
				for (int i = 0, n = input.ReadInt(true); i < n; i++)
					skin.constraints.Add(ikConstraintsItems[ikConstraintOffset + input.ReadInt(true)]);

				var transformConstraintsItems = skeletonData.transformConstraints.Items;
				for (int i = 0, n = input.ReadInt(true); i < n; i++)
					skin.constraints.Add(transformConstraintsItems[transformConstraintOffset + input.ReadInt(true)]);

				var pathConstraintsItems = skeletonData.pathConstraints.Items;
				for (int i = 0, n = input.ReadInt(true); i < n; i++)
					skin.constraints.Add(pathConstraintsItems[pathConstraintOffset + input.ReadInt(true)]);

				skin.constraints.TrimExcess();

				slotCount = input.ReadInt(true);
			}
			for (int i = 0; i < slotCount; i++) {
				int slotIndex = slotOffset + input.ReadInt(true);
				for (int ii = 0, nn = input.ReadInt(true); ii < nn; ii++) {
					String name = input.ReadStringRef(); 
					Attachment attachment = ReadAttachment(input, skeletonData, skin, slotIndex, name, nonessential, idx);
                    SlotData slotData = skeletonData.slots.Items[slotIndex];
					if (attachment != null) skin.SetAttachment(slotIndex, name, attachment);
				}
			}
			return skin;
		}

		private Attachment ReadAttachment (SkeletonInput input, SkeletonData skeletonData, Skin skin, int slotIndex, String attachmentName, bool nonessential, int idx = 1) {
			float scale = this.scale;

			String name = input.ReadStringRef();
			if (name == null) name = attachmentName;

			switch ((AttachmentType)input.ReadByte()) {
			case AttachmentType.Region: {
				String path = input.ReadStringRef();
				float rotation = input.ReadFloat();
				float x = input.ReadFloat();
				float y = input.ReadFloat();
				float scaleX = input.ReadFloat();
				float scaleY = input.ReadFloat();
				float width = input.ReadFloat();
				float height = input.ReadFloat();
				int color = input.ReadInt();

				if (path == null) path = name;
				RegionAttachment region = attachmentLoader.NewRegionAttachment(skin, name, path);
				if (region == null) return null;
				region.Path = path;
				region.x = x * scale;
				region.y = y * scale;
				region.scaleX = scaleX;
				region.scaleY = scaleY;
				region.rotation = rotation;
				region.width = width * scale;
				region.height = height * scale;
				region.r = ((color & 0xff000000) >> 24) / 255f;
				region.g = ((color & 0x00ff0000) >> 16) / 255f;
				region.b = ((color & 0x0000ff00) >> 8) / 255f;
				region.a = ((color & 0x000000ff)) / 255f;
				region.UpdateOffset();
				return region;
			}
			case AttachmentType.Boundingbox: {
				int vertexCount = input.ReadInt(true);
				Vertices vertices = ReadVertices(input, vertexCount, skeletonData, idx);
				if (nonessential) input.ReadInt(); //int color = nonessential ? input.ReadInt() : 0; // Avoid unused local warning.

				BoundingBoxAttachment box = attachmentLoader.NewBoundingBoxAttachment(skin, name);
				if (box == null) return null;
				box.worldVerticesLength = vertexCount << 1;
				box.vertices = vertices.vertices;
				box.bones = vertices.bones;
				// skipped porting: if (nonessential) Color.rgba8888ToColor(box.getColor(), color);
				return box;
			}
			case AttachmentType.Mesh: {
				String path = input.ReadStringRef();
				int color = input.ReadInt();
				int vertexCount = input.ReadInt(true);
				float[] uvs = ReadFloatArray(input, vertexCount << 1, 1);
				int[] triangles = ReadShortArray(input);
				Vertices vertices = ReadVertices(input, vertexCount, skeletonData, idx);
				int hullLength = input.ReadInt(true);
				int[] edges = null;
				float width = 0, height = 0;
				if (nonessential) {
					edges = ReadShortArray(input);
					width = input.ReadFloat();
					height = input.ReadFloat();
				}

				if (path == null) path = name;
				MeshAttachment mesh = attachmentLoader.NewMeshAttachment(skin, name, path);
				if (mesh == null) return null;
				mesh.Path = path;
				mesh.r = ((color & 0xff000000) >> 24) / 255f;
				mesh.g = ((color & 0x00ff0000) >> 16) / 255f;
				mesh.b = ((color & 0x0000ff00) >> 8) / 255f;
				mesh.a = ((color & 0x000000ff)) / 255f;
				mesh.bones = vertices.bones;
				mesh.vertices = vertices.vertices;
				mesh.WorldVerticesLength = vertexCount << 1;
				mesh.triangles = triangles;
				mesh.regionUVs = uvs;
				mesh.UpdateUVs();
				mesh.HullLength = hullLength << 1;
				if (nonessential) {
					mesh.Edges = edges;
					mesh.Width = width * scale;
					mesh.Height = height * scale;
				}
				return mesh;
			}
			case AttachmentType.Linkedmesh: {
				String path = input.ReadStringRef();
				int color = input.ReadInt();
				String skinName = input.ReadStringRef();
				String parent = input.ReadStringRef();
				bool inheritDeform = input.ReadBoolean();
				float width = 0, height = 0;
				if (nonessential) {
					width = input.ReadFloat();
					height = input.ReadFloat();
				}

				if (path == null) path = name;
				MeshAttachment mesh = attachmentLoader.NewMeshAttachment(skin, name, path);
				if (mesh == null) return null;
				mesh.Path = path;
				mesh.r = ((color & 0xff000000) >> 24) / 255f;
				mesh.g = ((color & 0x00ff0000) >> 16) / 255f;
				mesh.b = ((color & 0x0000ff00) >> 8) / 255f;
				mesh.a = ((color & 0x000000ff)) / 255f;
				if (nonessential) {
					mesh.Width = width * scale;
					mesh.Height = height * scale;
				}
				linkedMeshes.Add(new SkeletonMultiLoader.LinkedMesh(mesh, skinName, slotIndex, parent, inheritDeform));
				return mesh;
			}
			case AttachmentType.Path: {
				bool closed = input.ReadBoolean();
				bool constantSpeed = input.ReadBoolean();
				int vertexCount = input.ReadInt(true);
				Vertices vertices = ReadVertices(input, vertexCount, skeletonData, idx);
				float[] lengths = new float[vertexCount / 3];
				for (int i = 0, n = lengths.Length; i < n; i++)
					lengths[i] = input.ReadFloat() * scale;
				if (nonessential) input.ReadInt(); //int color = nonessential ? input.ReadInt() : 0;

				PathAttachment path = attachmentLoader.NewPathAttachment(skin, name);
				if (path == null) return null;
				path.closed = closed;
				path.constantSpeed = constantSpeed;
				path.worldVerticesLength = vertexCount << 1;
				path.vertices = vertices.vertices;
				path.bones = vertices.bones;
				path.lengths = lengths;
				// skipped porting: if (nonessential) Color.rgba8888ToColor(path.getColor(), color);
				return path;
			}
			case AttachmentType.Point: {
				float rotation = input.ReadFloat();
				float x = input.ReadFloat();
				float y = input.ReadFloat();
				if (nonessential) input.ReadInt(); //int color = nonessential ? input.ReadInt() : 0;

				PointAttachment point = attachmentLoader.NewPointAttachment(skin, name);
				if (point == null) return null;
				point.x = x * scale;
				point.y = y * scale;
				point.rotation = rotation;
				// skipped porting: if (nonessential) point.color = color;
				return point;
			}
			case AttachmentType.Clipping: {
				int endSlotIndex = input.ReadInt(true);
				int vertexCount = input.ReadInt(true);
				Vertices vertices = ReadVertices(input, vertexCount, skeletonData, idx);
				if (nonessential) input.ReadInt();

				ClippingAttachment clip = attachmentLoader.NewClippingAttachment(skin, name);
				if (clip == null) return null;
				clip.EndSlot = skeletonData.slots.Items[endSlotIndex];
				clip.worldVerticesLength = vertexCount << 1;
				clip.vertices = vertices.vertices;
				clip.bones = vertices.bones;
				// skipped porting: if (nonessential) Color.rgba8888ToColor(clip.getColor(), color);
				return clip;
			}
			}
			return null;
		}

		private Vertices ReadVertices (SkeletonInput input, int vertexCount, SkeletonData skeletonData, int idx) {
			float scale = this.scale;
			int verticesLength = vertexCount << 1;
			Vertices vertices = new Vertices();
			if (!input.ReadBoolean()) {
				vertices.vertices = ReadFloatArray(input, verticesLength, scale);
				return vertices;
			}
			var weights = new ExposedList<float>(verticesLength * 3 * 3);
			var bonesArray = new ExposedList<int>(verticesLength * 3);
            int boneOffset;
            switch (idx) {
                case 3:
                    boneOffset = skeletonData.boneOffset3;
                    break;

                case 2:
                    boneOffset = skeletonData.boneOffset2;
                    break;

                default:
                    boneOffset = 0;
                    break;
            }
			for (int i = 0; i < vertexCount; i++) {
				int boneCount = input.ReadInt(true);
				bonesArray.Add(boneCount);
				for (int ii = 0; ii < boneCount; ii++) {
					bonesArray.Add(boneOffset + input.ReadInt(true));
					weights.Add(input.ReadFloat() * scale);
					weights.Add(input.ReadFloat() * scale);
					weights.Add(input.ReadFloat());
				}
			}

			vertices.vertices = weights.ToArray();
			vertices.bones = bonesArray.ToArray();
			return vertices;
		}

		private float[] ReadFloatArray (SkeletonInput input, int n, float scale) {
			float[] array = new float[n];
			if (scale == 1) {
				for (int i = 0; i < n; i++)
					array[i] = input.ReadFloat();
			} else {
				for (int i = 0; i < n; i++)
					array[i] = input.ReadFloat() * scale;
			}
			return array;
		}

		private int[] ReadShortArray (SkeletonInput input) {
			int n = input.ReadInt(true);
			int[] array = new int[n];
			for (int i = 0; i < n; i++)
				array[i] = (input.ReadByte() << 8) | input.ReadByte();
			return array;
		}

		private void ReadSlotTimelines (SkeletonInput input, ExposedList<Timeline> timelines, int slotOffset) {
			for (int i = 0, n = input.ReadInt(true); i < n; i++) {
				int slotIndex = slotOffset + input.ReadInt(true);
				for (int ii = 0, nn = input.ReadInt(true); ii < nn; ii++) {
					int timelineType = input.ReadByte(), frameCount = input.ReadInt(true), frameLast = frameCount - 1;
					switch (timelineType) {
					case SLOT_ATTACHMENT: {
						AttachmentTimeline timeline = new AttachmentTimeline(frameCount, slotIndex);
						for (int frame = 0; frame < frameCount; frame++)
							timeline.SetFrame(frame, input.ReadFloat(), input.ReadStringRef());
						timelines.Add(timeline);
						break;
					}
					case SLOT_RGBA: {
						RGBATimeline timeline = new RGBATimeline(frameCount, input.ReadInt(true), slotIndex);
						float time = input.ReadFloat();
						float r = input.Read() / 255f, g = input.Read() / 255f;
						float b = input.Read() / 255f, a = input.Read() / 255f;
						for (int frame = 0, bezier = 0; ; frame++) {
							timeline.SetFrame(frame, time, r, g, b, a);
							if (frame == frameLast) break;
							float time2 = input.ReadFloat();
							float r2 = input.Read() / 255f, g2 = input.Read() / 255f;
							float b2 = input.Read() / 255f, a2 = input.Read() / 255f;
							switch (input.ReadByte()) {
							case CURVE_STEPPED:
								timeline.SetStepped(frame);
								break;
							case CURVE_BEZIER:
								SetBezier(input, timeline, bezier++, frame, 0, time, time2, r, r2, 1);
								SetBezier(input, timeline, bezier++, frame, 1, time, time2, g, g2, 1);
								SetBezier(input, timeline, bezier++, frame, 2, time, time2, b, b2, 1);
								SetBezier(input, timeline, bezier++, frame, 3, time, time2, a, a2, 1);
								break;
							}
							time = time2;
							r = r2;
							g = g2;
							b = b2;
							a = a2;
						}
						timelines.Add(timeline);
						break;
					}
					case SLOT_RGB: {
						RGBTimeline timeline = new RGBTimeline(frameCount, input.ReadInt(true), slotIndex);
						float time = input.ReadFloat();
						float r = input.Read() / 255f, g = input.Read() / 255f, b = input.Read() / 255f;
						for (int frame = 0, bezier = 0; ; frame++) {
							timeline.SetFrame(frame, time, r, g, b);
							if (frame == frameLast) break;
							float time2 = input.ReadFloat();
							float r2 = input.Read() / 255f, g2 = input.Read() / 255f, b2 = input.Read() / 255f;
							switch (input.ReadByte()) {
							case CURVE_STEPPED:
								timeline.SetStepped(frame);
								break;
							case CURVE_BEZIER:
								SetBezier(input, timeline, bezier++, frame, 0, time, time2, r, r2, 1);
								SetBezier(input, timeline, bezier++, frame, 1, time, time2, g, g2, 1);
								SetBezier(input, timeline, bezier++, frame, 2, time, time2, b, b2, 1);
								break;
							}
							time = time2;
							r = r2;
							g = g2;
							b = b2;
						}
						timelines.Add(timeline);
						break;
					}
					case SLOT_RGBA2: {
						RGBA2Timeline timeline = new RGBA2Timeline(frameCount, input.ReadInt(true), slotIndex);
						float time = input.ReadFloat();
						float r = input.Read() / 255f, g = input.Read() / 255f;
						float b = input.Read() / 255f, a = input.Read() / 255f;
						float r2 = input.Read() / 255f, g2 = input.Read() / 255f, b2 = input.Read() / 255f;
						for (int frame = 0, bezier = 0; ; frame++) {
							timeline.SetFrame(frame, time, r, g, b, a, r2, g2, b2);
							if (frame == frameLast) break;
							float time2 = input.ReadFloat();
							float nr = input.Read() / 255f, ng = input.Read() / 255f;
							float nb = input.Read() / 255f, na = input.Read() / 255f;
							float nr2 = input.Read() / 255f, ng2 = input.Read() / 255f, nb2 = input.Read() / 255f;
							switch (input.ReadByte()) {
							case CURVE_STEPPED:
								timeline.SetStepped(frame);
								break;
							case CURVE_BEZIER:
								SetBezier(input, timeline, bezier++, frame, 0, time, time2, r, nr, 1);
								SetBezier(input, timeline, bezier++, frame, 1, time, time2, g, ng, 1);
								SetBezier(input, timeline, bezier++, frame, 2, time, time2, b, nb, 1);
								SetBezier(input, timeline, bezier++, frame, 3, time, time2, a, na, 1);
								SetBezier(input, timeline, bezier++, frame, 4, time, time2, r2, nr2, 1);
								SetBezier(input, timeline, bezier++, frame, 5, time, time2, g2, ng2, 1);
								SetBezier(input, timeline, bezier++, frame, 6, time, time2, b2, nb2, 1);
								break;
							}
							time = time2;
							r = nr;
							g = ng;
							b = nb;
							a = na;
							r2 = nr2;
							g2 = ng2;
							b2 = nb2;
						}
						timelines.Add(timeline);
						break;
					}
					case SLOT_RGB2: {
						RGB2Timeline timeline = new RGB2Timeline(frameCount, input.ReadInt(true), slotIndex);
						float time = input.ReadFloat();
						float r = input.Read() / 255f, g = input.Read() / 255f, b = input.Read() / 255f;
						float r2 = input.Read() / 255f, g2 = input.Read() / 255f, b2 = input.Read() / 255f;
						for (int frame = 0, bezier = 0; ; frame++) {
							timeline.SetFrame(frame, time, r, g, b, r2, g2, b2);
							if (frame == frameLast) break;
							float time2 = input.ReadFloat();
							float nr = input.Read() / 255f, ng = input.Read() / 255f, nb = input.Read() / 255f;
							float nr2 = input.Read() / 255f, ng2 = input.Read() / 255f, nb2 = input.Read() / 255f;
							switch (input.ReadByte()) {
							case CURVE_STEPPED:
								timeline.SetStepped(frame);
								break;
							case CURVE_BEZIER:
								SetBezier(input, timeline, bezier++, frame, 0, time, time2, r, nr, 1);
								SetBezier(input, timeline, bezier++, frame, 1, time, time2, g, ng, 1);
								SetBezier(input, timeline, bezier++, frame, 2, time, time2, b, nb, 1);
								SetBezier(input, timeline, bezier++, frame, 3, time, time2, r2, nr2, 1);
								SetBezier(input, timeline, bezier++, frame, 4, time, time2, g2, ng2, 1);
								SetBezier(input, timeline, bezier++, frame, 5, time, time2, b2, nb2, 1);
								break;
							}
							time = time2;
							r = nr;
							g = ng;
							b = nb;
							r2 = nr2;
							g2 = ng2;
							b2 = nb2;
						}
						timelines.Add(timeline);
						break;
					}
					case SLOT_ALPHA: {
						AlphaTimeline timeline = new AlphaTimeline(frameCount, input.ReadInt(true), slotIndex);
						float time = input.ReadFloat(), a = input.Read() / 255f;
						for (int frame = 0, bezier = 0; ; frame++) {
							timeline.SetFrame(frame, time, a);
							if (frame == frameLast) break;
							float time2 = input.ReadFloat();
							float a2 = input.Read() / 255f;
							switch (input.ReadByte()) {
							case CURVE_STEPPED:
								timeline.SetStepped(frame);
								break;
							case CURVE_BEZIER:
								SetBezier(input, timeline, bezier++, frame, 0, time, time2, a, a2, 1);
								break;
							}
							time = time2;
							a = a2;
						}
						timelines.Add(timeline);
						break;
					}
					}
				}
			}
		}

		private void ReadBoneTimelines (SkeletonInput input, ExposedList<Timeline> timelines, int boneOffset) {
			for (int i = 0, n = input.ReadInt(true); i < n; i++) {
				int boneIndex = boneOffset + input.ReadInt(true);
				for (int ii = 0, nn = input.ReadInt(true); ii < nn; ii++) {
					int type = input.ReadByte(), frameCount = input.ReadInt(true), bezierCount = input.ReadInt(true);
					switch (type) {
					case BONE_ROTATE:
						timelines.Add(ReadTimeline(input, new RotateTimeline(frameCount, bezierCount, boneIndex), 1));
						break;
					case BONE_TRANSLATE:
						timelines.Add(ReadTimeline(input, new TranslateTimeline(frameCount, bezierCount, boneIndex), scale));
						break;
					case BONE_TRANSLATEX:
						timelines.Add(ReadTimeline(input, new TranslateXTimeline(frameCount, bezierCount, boneIndex), scale));
						break;
					case BONE_TRANSLATEY:
						timelines.Add(ReadTimeline(input, new TranslateYTimeline(frameCount, bezierCount, boneIndex), scale));
						break;
					case BONE_SCALE:
						timelines.Add(ReadTimeline(input, new ScaleTimeline(frameCount, bezierCount, boneIndex), 1));
						break;
					case BONE_SCALEX:
						timelines.Add(ReadTimeline(input, new ScaleXTimeline(frameCount, bezierCount, boneIndex), 1));
						break;
					case BONE_SCALEY:
						timelines.Add(ReadTimeline(input, new ScaleYTimeline(frameCount, bezierCount, boneIndex), 1));
						break;
					case BONE_SHEAR:
						timelines.Add(ReadTimeline(input, new ShearTimeline(frameCount, bezierCount, boneIndex), 1));
						break;
					case BONE_SHEARX:
						timelines.Add(ReadTimeline(input, new ShearXTimeline(frameCount, bezierCount, boneIndex), 1));
						break;
					case BONE_SHEARY:
						timelines.Add(ReadTimeline(input, new ShearYTimeline(frameCount, bezierCount, boneIndex), 1));
						break;
					}
				}
			}
		}

		// IK constraint timelines
		private void ReadIKConstraintTimelines (SkeletonInput input, ExposedList<Timeline> timelines, int ikConstraintOffset) {
			for (int i = 0, n = input.ReadInt(true); i < n; i++) {
				int index = ikConstraintOffset + input.ReadInt(true), frameCount = input.ReadInt(true), frameLast = frameCount - 1;
				IkConstraintTimeline timeline = new IkConstraintTimeline(frameCount, input.ReadInt(true), index);
				float time = input.ReadFloat(), mix = input.ReadFloat(), softness = input.ReadFloat() * scale;
				for (int frame = 0, bezier = 0; ; frame++) {
					timeline.SetFrame(frame, time, mix, softness, input.ReadSByte(), input.ReadBoolean(), input.ReadBoolean());
					if (frame == frameLast) break;
					float time2 = input.ReadFloat(), mix2 = input.ReadFloat(), softness2 = input.ReadFloat() * scale;
					switch (input.ReadByte()) {
					case CURVE_STEPPED:
						timeline.SetStepped(frame);
						break;
					case CURVE_BEZIER:
						SetBezier(input, timeline, bezier++, frame, 0, time, time2, mix, mix2, 1);
						SetBezier(input, timeline, bezier++, frame, 1, time, time2, softness, softness2, scale);
						break;
					}
					time = time2;
					mix = mix2;
					softness = softness2;
				}
				timelines.Add(timeline);
			}
		}

		// Transform constraint timelines.
		private void ReadTransformConstraintTimelines (SkeletonInput input, ExposedList<Timeline> timelines, int transformConstraintOffset) {
			for (int i = 0, n = input.ReadInt(true); i < n; i++) {
				int index = transformConstraintOffset + input.ReadInt(true), frameCount = input.ReadInt(true), frameLast = frameCount - 1;
				TransformConstraintTimeline timeline = new TransformConstraintTimeline(frameCount, input.ReadInt(true), index);
				float time = input.ReadFloat(), mixRotate = input.ReadFloat(), mixX = input.ReadFloat(), mixY = input.ReadFloat(),
				mixScaleX = input.ReadFloat(), mixScaleY = input.ReadFloat(), mixShearY = input.ReadFloat();
				for (int frame = 0, bezier = 0; ; frame++) {
					timeline.SetFrame(frame, time, mixRotate, mixX, mixY, mixScaleX, mixScaleY, mixShearY);
					if (frame == frameLast) break;
					float time2 = input.ReadFloat(), mixRotate2 = input.ReadFloat(), mixX2 = input.ReadFloat(), mixY2 = input.ReadFloat(),
					mixScaleX2 = input.ReadFloat(), mixScaleY2 = input.ReadFloat(), mixShearY2 = input.ReadFloat();
					switch (input.ReadByte()) {
					case CURVE_STEPPED:
						timeline.SetStepped(frame);
						break;
					case CURVE_BEZIER:
						SetBezier(input, timeline, bezier++, frame, 0, time, time2, mixRotate, mixRotate2, 1);
						SetBezier(input, timeline, bezier++, frame, 1, time, time2, mixX, mixX2, 1);
						SetBezier(input, timeline, bezier++, frame, 2, time, time2, mixY, mixY2, 1);
						SetBezier(input, timeline, bezier++, frame, 3, time, time2, mixScaleX, mixScaleX2, 1);
						SetBezier(input, timeline, bezier++, frame, 4, time, time2, mixScaleY, mixScaleY2, 1);
						SetBezier(input, timeline, bezier++, frame, 5, time, time2, mixShearY, mixShearY2, 1);
						break;
					}
					time = time2;
					mixRotate = mixRotate2;
					mixX = mixX2;
					mixY = mixY2;
					mixScaleX = mixScaleX2;
					mixScaleY = mixScaleY2;
					mixShearY = mixShearY2;
				}
				timelines.Add(timeline);
			}
		}

		// Path constraint timelines.
		private void ReadPathConstraintTimelines (SkeletonInput input, ExposedList<Timeline> timelines, SkeletonData skeletonData, int pathConstraintOffset) {
			for (int i = 0, n = input.ReadInt(true); i < n; i++) {
				int index = pathConstraintOffset + input.ReadInt(true);
				PathConstraintData data = skeletonData.pathConstraints.Items[index];
				for (int ii = 0, nn = input.ReadInt(true); ii < nn; ii++) {
					switch (input.ReadByte()) {
					case PATH_POSITION:
						timelines
							.Add(ReadTimeline(input, new PathConstraintPositionTimeline(input.ReadInt(true), input.ReadInt(true), index),
								data.positionMode == PositionMode.Fixed ? scale : 1));
						break;
					case PATH_SPACING:
						timelines
							.Add(ReadTimeline(input, new PathConstraintSpacingTimeline(input.ReadInt(true), input.ReadInt(true), index),
								data.spacingMode == SpacingMode.Length || data.spacingMode == SpacingMode.Fixed ? scale : 1));
						break;
					case PATH_MIX:
						PathConstraintMixTimeline timeline = new PathConstraintMixTimeline(input.ReadInt(true), input.ReadInt(true),
							index);
						float time = input.ReadFloat(), mixRotate = input.ReadFloat(), mixX = input.ReadFloat(), mixY = input.ReadFloat();
						for (int frame = 0, bezier = 0, frameLast = timeline.FrameCount - 1; ; frame++) {
							timeline.SetFrame(frame, time, mixRotate, mixX, mixY);
							if (frame == frameLast) break;
							float time2 = input.ReadFloat(), mixRotate2 = input.ReadFloat(), mixX2 = input.ReadFloat(),
								mixY2 = input.ReadFloat();
							switch (input.ReadByte()) {
							case CURVE_STEPPED:
								timeline.SetStepped(frame);
								break;
							case CURVE_BEZIER:
								SetBezier(input, timeline, bezier++, frame, 0, time, time2, mixRotate, mixRotate2, 1);
								SetBezier(input, timeline, bezier++, frame, 1, time, time2, mixX, mixX2, 1);
								SetBezier(input, timeline, bezier++, frame, 2, time, time2, mixY, mixY2, 1);
								break;
							}
							time = time2;
							mixRotate = mixRotate2;
							mixX = mixX2;
							mixY = mixY2;
						}
						timelines.Add(timeline);
						break;
					}
				}
			}
		}

		// Deform timelines.
		private void ReadDeformTimelines (SkeletonInput input, ExposedList<Timeline> timelines, SkeletonData skeletonData, int slotOffset) {
			for (int i = 0, n = input.ReadInt(true); i < n; i++) {
				//  这里没有偏移skinOffset因为只支持一套皮肤
				Skin skin = skeletonData.skins.Items[input.ReadInt(true)];
				for (int ii = 0, nn = input.ReadInt(true); ii < nn; ii++) {
					int slotIndex = slotOffset + input.ReadInt(true);
					for (int iii = 0, nnn = input.ReadInt(true); iii < nnn; iii++) {
						String attachmentName = input.ReadStringRef();
						VertexAttachment attachment = (VertexAttachment)skin.GetAttachment(slotIndex, attachmentName);
						if (attachment == null) throw new SerializationException("Vertex attachment not found: " + attachmentName);
						bool weighted = attachment.Bones != null;
						float[] vertices = attachment.Vertices;
						int deformLength = weighted ? (vertices.Length / 3) << 1 : vertices.Length;

						int frameCount = input.ReadInt(true), frameLast = frameCount - 1;
						DeformTimeline timeline = new DeformTimeline(frameCount, input.ReadInt(true), slotIndex, attachment);

						float time = input.ReadFloat();
						for (int frame = 0, bezier = 0; ; frame++) {
							float[] deform;
							int end = input.ReadInt(true);
							if (end == 0)
								deform = weighted ? new float[deformLength] : vertices;
							else {
								deform = new float[deformLength];
								int start = input.ReadInt(true);
								end += start;
								if (scale == 1) {
									for (int v = start; v < end; v++)
										deform[v] = input.ReadFloat();
								} else {
									for (int v = start; v < end; v++)
										deform[v] = input.ReadFloat() * scale;
								}
								if (!weighted) {
									for (int v = 0, vn = deform.Length; v < vn; v++)
										deform[v] += vertices[v];
								}
							}
							timeline.SetFrame(frame, time, deform);
							if (frame == frameLast) break;
							float time2 = input.ReadFloat();
							switch (input.ReadByte()) {
							case CURVE_STEPPED:
								timeline.SetStepped(frame);
								break;
							case CURVE_BEZIER:
								SetBezier(input, timeline, bezier++, frame, 0, time, time2, 0, 1, 1);
								break;
							}
							time = time2;
						}
						timelines.Add(timeline);
					}
				}
			}
		}

		// Event timeline.
		private void ReadEventTimelines (SkeletonInput input, ExposedList<Timeline> timelines, SkeletonData skeletonData, int eventOffset) {
			int eventCount = input.ReadInt(true);
			if (eventCount > 0) {
				EventTimeline timeline = new EventTimeline(eventCount);
				for (int i = 0; i < eventCount; i++) {
					float time = input.ReadFloat();
					EventData eventData = skeletonData.events.Items[eventOffset + input.ReadInt(true)];
					Event e = new Event(time, eventData);
					e.intValue = input.ReadInt(false);
					e.floatValue = input.ReadFloat();
					e.stringValue = input.ReadBoolean() ? input.ReadString() : eventData.String;
					if (e.Data.AudioPath != null) {
						e.volume = input.ReadFloat();
						e.balance = input.ReadFloat();
					}
					timeline.SetFrame(i, e);
				}
				timelines.Add(timeline);
			}
		}

		/// <exception cref="SerializationException">SerializationException will be thrown when a Vertex attachment is not found.</exception>
		/// <exception cref="IOException">Throws IOException when a read operation fails.</exception>
		private Animation ReadAnimation (String name, SkeletonInput input, SkeletonInput input2, SkeletonInput input3, SkeletonData skeletonData) {
			var timelineCount = input.ReadInt(true);
			var timelineCount2 = input2.ReadInt(true);
			var timelineCount3 = input3.ReadInt(true);

			var timelines = new ExposedList<Timeline>(timelineCount + timelineCount2 + timelineCount3);

			float scale = this.scale;

			// Slot timelines.
			this.ReadSlotTimelines(input, timelines, 0);
			this.ReadSlotTimelines(input2, timelines, skeletonData.slotOffset2);
			this.ReadSlotTimelines(input3, timelines, skeletonData.slotOffset3);
			
			// Bone timelines.
			this.ReadBoneTimelines(input, timelines, 0);
			this.ReadBoneTimelines(input2, timelines, skeletonData.boneOffset2);
			this.ReadBoneTimelines(input3, timelines, skeletonData.boneOffset3);

			// IK constraint timelines.
			this.ReadIKConstraintTimelines(input, timelines, 0);
			this.ReadIKConstraintTimelines(input2, timelines, skeletonData.ikConstraintOffset2);
			this.ReadIKConstraintTimelines(input3, timelines, skeletonData.ikConstraintOffset3);

			// Transform constraint timelines.
			this.ReadTransformConstraintTimelines(input, timelines, 0);
			this.ReadTransformConstraintTimelines(input2, timelines, skeletonData.transformConstraintOffset2);
			this.ReadTransformConstraintTimelines(input3, timelines, skeletonData.transformConstraintOffset3);

			// Path constraint timelines.
			this.ReadPathConstraintTimelines(input, timelines, skeletonData, 0);
			this.ReadPathConstraintTimelines(input2, timelines, skeletonData, skeletonData.pathConstraintOffset2);
			this.ReadPathConstraintTimelines(input3, timelines, skeletonData, skeletonData.pathConstraintOffset3);

			// Deform timelines.
			this.ReadDeformTimelines(input, timelines, skeletonData, 0);
			this.ReadDeformTimelines(input2, timelines, skeletonData, skeletonData.slotOffset2);
			this.ReadDeformTimelines(input3, timelines, skeletonData, skeletonData.slotOffset3);
			

			// Draw order timeline.
            // TODO: 需要处理draworder
			int drawOrderCount = input.ReadInt(true);
			int drawOrderCount2 = input2.ReadInt(true);
			int drawOrderCount3 = input3.ReadInt(true);

			Dictionary<float, Object> valueMap = new Dictionary<float, Object>();
			if (drawOrderCount > 0 || drawOrderCount2 > 0 || drawOrderCount3 > 0) {
				for (int i = 0; i < drawOrderCount; i++) {
					float time = input.ReadFloat();
					Dictionary<string, Object> drawOrderMap;
					if (valueMap.ContainsKey(time)) {
						drawOrderMap = (Dictionary<string, Object>)valueMap[time];
					} else {
						drawOrderMap = new Dictionary<string, Object>();
						drawOrderMap.Add("time", time);
						valueMap.Add(time, drawOrderMap);
					}
					List<Object> offsets = new List<Object>();
					int offsetCount = input.ReadInt(true);
					for (int ii = 0; ii < offsetCount; ii++) {
						int slotIndex = input.ReadInt(true);
						int offset = input.ReadInt(true);
						Dictionary<string, Object> offsetMap = new Dictionary<string, Object>();
						offsetMap.Add("slotIndex", slotIndex);
						offsetMap.Add("offset", offset);
						offsets.Add(offsetMap);
					}

					if (drawOrderMap.ContainsKey("offsets")) {
						((List<Object>)drawOrderMap["offsets"]).AddRange(offsets);
					} else {
						drawOrderMap.Add("offsets", offsets);
					}
				}

				for (int i = 0; i < drawOrderCount2; i++) {
					float time = input2.ReadFloat();
					Dictionary<string, Object> drawOrderMap;
					if (valueMap.ContainsKey(time)) {
						drawOrderMap = (Dictionary<string, Object>)valueMap[time];
					} else {
						drawOrderMap = new Dictionary<string, Object>();
						drawOrderMap.Add("time", time);
						valueMap.Add(time, drawOrderMap);
					}
					List<Object> offsets = new List<Object>();
					int offsetCount = input2.ReadInt(true);
					for (int ii = 0; ii < offsetCount; ii++) {
						int slotIndex = skeletonData.slotOffset2 + input2.ReadInt(true);
						int offset = input2.ReadInt(true);
						Dictionary<string, Object> offsetMap = new Dictionary<string, Object>();
						offsetMap.Add("slotIndex", slotIndex);
						offsetMap.Add("offset", offset);
						offsets.Add(offsetMap);
					}

					if (drawOrderMap.ContainsKey("offsets")) {
						((List<Object>)drawOrderMap["offsets"]).AddRange(offsets);
					} else {
						drawOrderMap.Add("offsets", offsets);
					}
				}

				for (int i = 0; i < drawOrderCount3; i++) {
					float time = input3.ReadFloat();
					Dictionary<string, Object> drawOrderMap;
					if (valueMap.ContainsKey(time)) {
						drawOrderMap = (Dictionary<string, Object>)valueMap[time];
					} else {
						drawOrderMap = new Dictionary<string, Object>();
						drawOrderMap.Add("time", time);
						valueMap.Add(time, drawOrderMap);
					}
					List<Object> offsets = new List<Object>();
					int offsetCount = input3.ReadInt(true);
					for (int ii = 0; ii < offsetCount; ii++) {
						int slotIndex = skeletonData.slotOffset3 + input3.ReadInt(true);
						int offset = input3.ReadInt(true);
						Dictionary<string, Object> offsetMap = new Dictionary<string, Object>();
						offsetMap.Add("slotIndex", slotIndex);
						offsetMap.Add("offset", offset);
						offsets.Add(offsetMap);
					}
					if (drawOrderMap.ContainsKey("offsets")) {
						((List<Object>)drawOrderMap["offsets"]).AddRange(offsets);
					} else {
						drawOrderMap.Add("offsets", offsets);
					}
				}
			}

			if (!valueMap.ContainsKey(0F)) {
				Dictionary<string, Object> drawOrderMap = new Dictionary<string, Object>();
				drawOrderMap.Add("time", 0F);
				valueMap.Add(0F, drawOrderMap);
			}
			
			if (valueMap.Count > 0) {
				List<Object> values = new List<Object>();
				foreach (var kv in valueMap)
				{
					values.Add(kv.Value);
				}

				values.Sort((obj1, obj2)=>{
					var time1 = GetFloat((Dictionary<string, Object>)obj1, "time", 0F);
					var time2 = GetFloat((Dictionary<string, Object>)obj2, "time", 0F);
					return time1.CompareTo(time2);
				});

				var timeline = new DrawOrderTimeline(values.Count);
				int slotCount = skeletonData.slots.Count;
				int frame = 0;
				UnityEngine.Debug.Log("slotCount:" + slotCount + " DrawOrderTimeline frame:" + values.Count);
				foreach (Dictionary<string, Object> drawOrderMap in values) {
					int[] drawOrder = null;
					if (drawOrderMap.ContainsKey("offsets")) {
						drawOrder = new int[slotCount];
						for (int i = slotCount - 1; i >= 0; i--)
							drawOrder[i] = -1;
						var offsets = (List<Object>)drawOrderMap["offsets"];
						int[] unchanged = new int[slotCount - offsets.Count];
						UnityEngine.Debug.Log("offsets.Count:" + offsets.Count + " unchanged.Length:" + unchanged.Length);
						int originalIndex = 0, unchangedIndex = 0;
						foreach (Dictionary<string, Object> offsetMap in offsets) {
							int slotIndex = (int)offsetMap["slotIndex"];
							// Collect unchanged items.
							UnityEngine.Debug.Log("slotIndex:" + slotIndex);
							while (originalIndex != slotIndex) {
								UnityEngine.Debug.Log("unchangedIndex:" + unchangedIndex);
								unchanged[unchangedIndex++] = originalIndex++;
							}
							
							// Set changed items.
							int index = originalIndex + (int)offsetMap["offset"];
							drawOrder[index] = originalIndex++;
						}
						// Collect remaining unchanged items.
						while (originalIndex < slotCount)
							unchanged[unchangedIndex++] = originalIndex++;
						// Fill in unchanged items.
						for (int i = slotCount - 1; i >= 0; i--)
							if (drawOrder[i] == -1) drawOrder[i] = unchanged[--unchangedIndex];
					} else {
						drawOrder = new int[slotCount];
						for (int i = slotCount - 1; i >= 0; i--)
							drawOrder[i] = i;
					}

					// 处理后发
					int[] newDrawOrder1 = new int[drawOrder.Length];
					int[] newDrawOrder2 = new int[drawOrder.Length];
					for (int i = drawOrder.Length - 1; i >= 0; i--) {
						newDrawOrder1[i] = -1;
						newDrawOrder2[i] = -1;
					}

					int idx1 = 0;
					int idx2 = 0;
					SlotData[] slots = skeletonData.slots.Items;
					for (int i = 0; i < drawOrder.Length; i++) {
						int slotIndex = drawOrder[i];
						if (slots[slotIndex].name.Contains("_keybottom")) {
							newDrawOrder1[idx1++] = slotIndex;
						} else {
							newDrawOrder2[idx2++] = slotIndex;
						}
					}

					int idx = 0;
					for (int i = 0; i < idx1; i++) {
						drawOrder[idx++] = newDrawOrder1[i];
					}
					for (int i = 0; i < idx2; i++) {
						drawOrder[idx++] = newDrawOrder2[i];
					}

					timeline.SetFrame(frame, GetFloat(drawOrderMap, "time", 0), drawOrder);
					++frame;
				}

				timelines.Add(timeline);
			}

			// Event timeline.
			this.ReadEventTimelines(input, timelines, skeletonData, 0);
			this.ReadEventTimelines(input2, timelines, skeletonData, skeletonData.eventOffset2);
			this.ReadEventTimelines(input3, timelines, skeletonData, skeletonData.eventOffset3);

			float duration = 0;
			var items = timelines.Items;

			for (int i = 0, n = timelines.Count; i < n; i++) {
				duration = Math.Max(duration, items[i].Duration);
            }

			return new Animation(name, timelines, duration);
		}

		/// <exception cref="IOException">Throws IOException when a read operation fails.</exception>
		private Timeline ReadTimeline (SkeletonInput input, CurveTimeline1 timeline, float scale) {
			float time = input.ReadFloat(), value = input.ReadFloat() * scale;
			for (int frame = 0, bezier = 0, frameLast = timeline.FrameCount - 1; ; frame++) {
				timeline.SetFrame(frame, time, value);
				if (frame == frameLast) break;
				float time2 = input.ReadFloat(), value2 = input.ReadFloat() * scale;
				switch (input.ReadByte()) {
				case CURVE_STEPPED:
					timeline.SetStepped(frame);
					break;
				case CURVE_BEZIER:
					SetBezier(input, timeline, bezier++, frame, 0, time, time2, value, value2, scale);
					break;
				}
				time = time2;
				value = value2;
			}
			return timeline;
		}

		/// <exception cref="IOException">Throws IOException when a read operation fails.</exception>
		private Timeline ReadTimeline (SkeletonInput input, CurveTimeline2 timeline, float scale) {
			float time = input.ReadFloat(), value1 = input.ReadFloat() * scale, value2 = input.ReadFloat() * scale;
			for (int frame = 0, bezier = 0, frameLast = timeline.FrameCount - 1; ; frame++) {
				timeline.SetFrame(frame, time, value1, value2);
				if (frame == frameLast) break;
				float time2 = input.ReadFloat(), nvalue1 = input.ReadFloat() * scale, nvalue2 = input.ReadFloat() * scale;
				switch (input.ReadByte()) {
				case CURVE_STEPPED:
					timeline.SetStepped(frame);
					break;
				case CURVE_BEZIER:
					SetBezier(input, timeline, bezier++, frame, 0, time, time2, value1, nvalue1, scale);
					SetBezier(input, timeline, bezier++, frame, 1, time, time2, value2, nvalue2, scale);
					break;
				}
				time = time2;
				value1 = nvalue1;
				value2 = nvalue2;
			}
			return timeline;
		}

		/// <exception cref="IOException">Throws IOException when a read operation fails.</exception>
		void SetBezier (SkeletonInput input, CurveTimeline timeline, int bezier, int frame, int value, float time1, float time2,
			float value1, float value2, float scale) {
			timeline.SetBezier(bezier, frame, value, time1, value1, input.ReadFloat(), input.ReadFloat() * scale, input.ReadFloat(),
					input.ReadFloat() * scale, time2, value2);
		}

		static float GetFloat (Dictionary<string, Object> map, string name, float defaultValue) {
			if (!map.ContainsKey(name)) return defaultValue;
			return (float)map[name];
		}

		internal class Vertices {
			public int[] bones;
			public float[] vertices;
		}

		internal class SkeletonInput {
			private byte[] chars = new byte[32];
			private byte[] bytesBigEndian = new byte[8];
			internal string[] strings;
			Stream input;

			public SkeletonInput (Stream input) {
				this.input = input;
			}

			public int Read () {
				return input.ReadByte();
			}

			public byte ReadByte () {
				return (byte)input.ReadByte();
			}

			public sbyte ReadSByte () {
				int value = input.ReadByte();
				if (value == -1) throw new EndOfStreamException();
				return (sbyte)value;
			}

			public bool ReadBoolean () {
				return input.ReadByte() != 0;
			}

			public float ReadFloat () {
				input.Read(bytesBigEndian, 0, 4);
				chars[3] = bytesBigEndian[0];
				chars[2] = bytesBigEndian[1];
				chars[1] = bytesBigEndian[2];
				chars[0] = bytesBigEndian[3];
				return BitConverter.ToSingle(chars, 0);
			}

			public int ReadInt () {
				input.Read(bytesBigEndian, 0, 4);
				return (bytesBigEndian[0] << 24)
					+ (bytesBigEndian[1] << 16)
					+ (bytesBigEndian[2] << 8)
					+ bytesBigEndian[3];
			}

			public long ReadLong () {
				input.Read(bytesBigEndian, 0, 8);
				return ((long)(bytesBigEndian[0]) << 56)
					+ ((long)(bytesBigEndian[1]) << 48)
					+ ((long)(bytesBigEndian[2]) << 40)
					+ ((long)(bytesBigEndian[3]) << 32)
					+ ((long)(bytesBigEndian[4]) << 24)
					+ ((long)(bytesBigEndian[5]) << 16)
					+ ((long)(bytesBigEndian[6]) << 8)
					+ (long)(bytesBigEndian[7]);
			}

			public int ReadInt (bool optimizePositive) {
				int b = input.ReadByte();
				int result = b & 0x7F;
				if ((b & 0x80) != 0) {
					b = input.ReadByte();
					result |= (b & 0x7F) << 7;
					if ((b & 0x80) != 0) {
						b = input.ReadByte();
						result |= (b & 0x7F) << 14;
						if ((b & 0x80) != 0) {
							b = input.ReadByte();
							result |= (b & 0x7F) << 21;
							if ((b & 0x80) != 0) result |= (input.ReadByte() & 0x7F) << 28;
						}
					}
				}
				return optimizePositive ? result : ((result >> 1) ^ -(result & 1));
			}

			public string ReadString () {
				int byteCount = ReadInt(true);
				switch (byteCount) {
				case 0:
					return null;
				case 1:
					return "";
				}
				byteCount--;
				byte[] buffer = this.chars;
				if (buffer.Length < byteCount) buffer = new byte[byteCount];
				ReadFully(buffer, 0, byteCount);
				return System.Text.Encoding.UTF8.GetString(buffer, 0, byteCount);
			}

			///<return>May be null.</return>
			public String ReadStringRef () {
				int index = ReadInt(true);
				return index == 0 ? null : strings[index - 1];
			}

			public void ReadFully (byte[] buffer, int offset, int length) {
				while (length > 0) {
					int count = input.Read(buffer, offset, length);
					if (count <= 0) throw new EndOfStreamException();
					offset += count;
					length -= count;
				}
			}

			/// <summary>Returns the version string of binary skeleton data.</summary>
			public string GetVersionString () {
				try {
					// try reading 4.0+ format
					var initialPosition = input.Position;
					ReadLong(); // long hash

					var stringPosition = input.Position;
					int stringByteCount = ReadInt(true);
					input.Position = stringPosition;
					if (stringByteCount <= 13) {
						string version = ReadString();
						if (char.IsDigit(version[0]))
							return version;
					}
					// fallback to old version format
					input.Position = initialPosition;
					return GetVersionStringOld3X();
				} catch (Exception e) {
					throw new ArgumentException("Stream does not contain valid binary Skeleton Data.\n" + e, "input");
				}
			}

			/// <summary>Returns old 3.8 and earlier format version string of binary skeleton data.</summary>
			public string GetVersionStringOld3X () {
				// Hash.
				int byteCount = ReadInt(true);
				if (byteCount > 1) input.Position += byteCount - 1;

				// Version.
				byteCount = ReadInt(true);
				if (byteCount > 1 && byteCount <= 13) {
					byteCount--;
					var buffer = new byte[byteCount];
					ReadFully(buffer, 0, byteCount);
					return System.Text.Encoding.UTF8.GetString(buffer, 0, byteCount);
				}
				throw new ArgumentException("Stream does not contain valid binary Skeleton Data.");
			}
		}
	}
}
