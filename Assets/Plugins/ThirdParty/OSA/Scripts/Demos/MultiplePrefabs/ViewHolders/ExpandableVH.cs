using System;
using frame8.Logic.Misc.Other.Extensions;
using Com.TheFallenGames.OSA.Util;
using Com.TheFallenGames.OSA.Util.IO;
using Com.TheFallenGames.OSA.Demos.MultiplePrefabs.Models;

namespace Com.TheFallenGames.OSA.Demos.MultiplePrefabs.ViewsHolders
{
    /// <summary>The views holder that can preset an <see cref="ExpandableModel"/>. It demonstrates the flow of data both from the view to the model and vice-versa</summary>
    public class ExpandableVH : BaseVH
    {
        public RemoteImageBehaviour remoteImageBehaviour;
        public ExpandCollapseOnClick expandCollapseOnClickBehaviour;


		/// <inheritdoc/>
		public override void CollectViews()
        {
            base.CollectViews();

			root.GetComponentAtPath("SimpleAvatarPanel/TitleText", out titleText);
			root.GetComponentAtPath("SimpleAvatarPanel/Panel/MaskWithImage/IconRawImage", out remoteImageBehaviour);

			expandCollapseOnClickBehaviour = root.GetComponent<ExpandCollapseOnClick>();
        }

        /// <summary>Can only preset models of type <see cref="ExpandableModel"/></summary>
        public override bool CanPresentModelType(Type modelType) { return modelType == typeof(ExpandableModel); }

		/// <inheritdoc/>
		public override void UpdateViews(BaseModel model)
        {
            base.UpdateViews(model);

            var modelAsExpandable = model as ExpandableModel;
            remoteImageBehaviour.Load(modelAsExpandable.imageURL);

            // Modify the recycled expand behavior script so it's up-to-date with the model. 
            if (expandCollapseOnClickBehaviour)
            {
                expandCollapseOnClickBehaviour.expanded = modelAsExpandable.expanded;
                expandCollapseOnClickBehaviour.nonExpandedSize = modelAsExpandable.nonExpandedSize;
            }
        }
    }
}
