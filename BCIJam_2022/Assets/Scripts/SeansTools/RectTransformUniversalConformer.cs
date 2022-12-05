using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Shapes;

#if UNITY_EDITOR
using UnityEditor;
#endif


[RequireComponent(typeof(RectTransform))]
[ExecuteInEditMode]
public class RectTransformUniversalConformer : MonoBehaviour {
	private bool initialized = false;
	public bool updateAtRuntime = true;
	public bool updateOnValidate = false;
	public bool propagateToChildren = false;
	public bool debugUpdateRealtimeEditor = false;
	//public bool pivotFromCorner = false;
	[PropertySpace(SpaceBefore = 0, SpaceAfter = 10)]
	[SerializeField] bool debug = false;

	RectTransform o_Rect;
	Rect lastConfiguration;
	Sprite lastSprite;

	Polygon o_ShapesPolygon;
	Rectangle o_ShapesRectangle;
	Disc o_ShapesDisc;
	Line o_ShapesLine;
	BoxCollider2D o_BoxCollider;
	CircleCollider2D o_CircleCollider;
	SpriteRenderer o_SpriteRenderer;
	ParticleSystem o_ParticleSystem;

	bool hasShapesPolygon = false;
	bool hasShapesRect = false;
	bool hasShapesDisc = false;
	bool hasShapesLine = false;
	bool hasBoxCollider = false;
	bool hasCircleCollider = false;
	bool hasSpriteRenderer = false;
	bool hasParticleSystem = false;

	private void VerifyInitialized() {
		if(initialized) return;
		o_Rect = GetComponent<RectTransform>();

		o_ShapesPolygon = GetComponent<Polygon>();
		hasShapesPolygon = (o_ShapesPolygon != null);
		if(!hasShapesPolygon) affectShapesPolygon = false;

		o_ShapesRectangle = GetComponent<Rectangle>();
		hasShapesRect = (o_ShapesRectangle != null);
		if(!hasShapesRect) affectShapesRectangle = false;

		o_ShapesDisc = GetComponent<Disc>();
		hasShapesDisc = (o_ShapesDisc != null);
		if(!hasShapesDisc) affectShapesDisc = false;

		o_BoxCollider = GetComponent<BoxCollider2D>();
		hasBoxCollider = (o_BoxCollider != null);
		if(!hasBoxCollider) affectBoxCollider = false;

		o_CircleCollider = GetComponent<CircleCollider2D>();
		hasCircleCollider = (o_CircleCollider != null);
		if(!hasCircleCollider) affectCircleCollider = false;

		o_SpriteRenderer = GetComponent<SpriteRenderer>();
		hasSpriteRenderer = (o_SpriteRenderer != null);
		if(!hasSpriteRenderer) affectSpriteRenderer = false;

		o_ParticleSystem = GetComponent<ParticleSystem>();
		hasParticleSystem = (o_ParticleSystem != null);
		if(!hasParticleSystem) affectParticleSystem = false;

		o_ShapesLine = GetComponent<Line>();
		hasShapesLine = (o_ShapesLine != null);
		if(!hasShapesLine) affectShapesLine = false;

		initialized = true;
	}

	public void LateUpdate() {
		if((updateAtRuntime && Application.isPlaying) || (debugUpdateRealtimeEditor)) {
			if(o_Rect.rect != lastConfiguration) {
				VerifyInitialized();
				ConformAll(false);
			}
			else if(affectSpriteRenderer && o_SpriteRenderer.sprite != lastSprite) {
				VerifyInitialized();
				ConformAll(false);
			}
		}
	}

	public void Awake() {
		VerifyInitialized();
		if(updateAtRuntime && Application.isPlaying) {
			ConformAll(false);
		}
	}

	[ShowIf("hasShapesPolygon")]
	[FoldoutGroup("Affected components")]
	public bool affectShapesPolygon = false;

	[ShowIf("hasShapesRect")]
	[FoldoutGroup("Affected components")]
	public bool affectShapesRectangle = false;

	[ShowIf("hasShapesDisc")]
	[FoldoutGroup("Affected components")]
	public bool affectShapesDisc = false;

	[ShowIf("hasShapesLine")]
	[FoldoutGroup("Affected components")]
	public bool affectShapesLine = false;

	[ShowIf("hasBoxCollider")]
	[FoldoutGroup("Affected components")]
	public bool affectBoxCollider = false;

	[ShowIf("hasCircleCollider")]
	[FoldoutGroup("Affected components")]
	public bool affectCircleCollider = false;

	[ShowIf("hasSpriteRenderer")]
	[FoldoutGroup("Affected components")]
	public bool affectSpriteRenderer = false;

	[ShowIf("hasParticleSystem")]
	[FoldoutGroup("Affected components")]
	public bool affectParticleSystem = false;

	[ShowIf("affectShapesPolygon")]
	public bool polygonOriginIsCorner = true;
	[ShowIf("affectShapesPolygon")]
	public List<Vector2> polyPoints;

	[ShowIf("hasShapesRect")]
	public bool affectShapesRectangleGradient = false;
	[ShowIf("affectShapesRectangleGradient")]
	public LinearGradientDirection linearGradientDirection;
	[ShowIf("affectShapesRectangleGradient")]
	public float radialGradientNormalizedSize;

	[ShowIf("hasShapesRect")]
	public bool affectShapesRectangleCorners = false;
	[ShowIf("affectShapesRectangleCorners")]
	public float cornerRadiusUL;
	[ShowIf("affectShapesRectangleCorners")]
	public float cornerRadiusUR;
	[ShowIf("affectShapesRectangleCorners")]
	public float cornerRadiusLL;
	[ShowIf("affectShapesRectangleCorners")]
	public float cornerRadiusLR;

	[ShowIf("hasShapesLine")]
	public bool controlLineThickness = false;
	[ShowIf("controlLineThickness")]
	public float lineThickness;

	[ShowIf("hasShapesLine")]
	public bool controlLinePosition = false;
	[ShowIf("controlLinePosition")]
	public Vector2 lineAnchorStart;
	[ShowIf("controlLinePosition")]
	public Vector2 lineAnchorEnd;
	[ShowIf("controlLinePosition")]
	public int pixelOffsetX;
	[ShowIf("controlLinePosition")]
	public int pixelOffsetY;

	[ShowIf("affectSpriteRenderer")]
	public bool preserveSpriteAspect;

	[ShowIf("affectParticleSystem")]
	public float particleScale = 1.0f;

	[ShowIf("affectShapesDisc")]
	public bool controlDiscRadius;
	[ShowIf("affectShapesDisc")]
	public bool controlDiscThickness;
	[ShowIf("controlDiscThickness")]
	public float discThicknessScale;


	public void OnValidate() {
		o_ShapesRectangle = GetComponent<Rectangle>();
		hasShapesRect = (o_ShapesRectangle != null);

		o_ShapesDisc = GetComponent<Disc>();
		hasShapesDisc = (o_ShapesDisc != null);

		o_BoxCollider = GetComponent<BoxCollider2D>();
		hasBoxCollider = (o_BoxCollider != null);

		o_CircleCollider = GetComponent<CircleCollider2D>();
		hasCircleCollider = (o_CircleCollider != null);

		o_SpriteRenderer = GetComponent<SpriteRenderer>();
		hasSpriteRenderer = (o_SpriteRenderer != null);

		o_ParticleSystem = GetComponent<ParticleSystem>();
		hasParticleSystem = (o_ParticleSystem != null);

		//if(updateOnValidate) {
		//	ConformAll(true);
		//}
	}

	[Button("Conform")]
	public void ConformButtonPressed() {
		ConformAll(false);
	}

	public void ConformAll(bool calledOnValidate) {
		o_Rect = GetComponent<RectTransform>();

		if(hasShapesPolygon && affectShapesPolygon) {
			ConformShapesPolygon(o_ShapesPolygon);
		}

		if(hasShapesRect && affectShapesRectangle) {
			ConformShapesRectangle(o_ShapesRectangle);
		}

		if(hasShapesDisc && affectShapesDisc) {
			ConformShapesDisc(o_ShapesDisc);
		}

		if(hasShapesLine && affectShapesLine) {
			ConformShapesLine(o_ShapesLine);
		}

		if(hasBoxCollider && affectBoxCollider) {
			ConformBoxCollider(o_BoxCollider);
		}

		if(hasCircleCollider && affectCircleCollider) {
			ConformCircleCollider(o_CircleCollider);
		}

		if(hasSpriteRenderer && affectSpriteRenderer) {
			ConformSpriteRenderer(o_SpriteRenderer);
		}

		if(hasParticleSystem && affectParticleSystem) {
			ConformParticleSystem(o_ParticleSystem);
		}

		if(propagateToChildren) {
			RectTransformUniversalConformer[] childConformers = GetComponentsInChildren<RectTransformUniversalConformer>();
			foreach(RectTransformUniversalConformer conformer in childConformers) {
				if(conformer != this) conformer.ConformAll(calledOnValidate);
			}

			//if(Application.isPlaying) { //Only do this automatically in play mode since polygon.Process() can destroy objects, which should not happen via OnValidate().
			//	FulcrumPolygonOutliner[] childPolygons = GetComponentsInChildren<FulcrumPolygonOutliner>();
			//	foreach(FulcrumPolygonOutliner polygon in childPolygons) {
			//		if(polygon != this) polygon.Process(calledOnValidate);
			//	}
			//}
		}

#if UNITY_EDITOR
		EditorUtility.SetDirty(gameObject);
#endif
		if(debug) Debug.Log("RectTransformConformer updated.");
		lastConfiguration = o_Rect.rect;
	}

	public void ConformShapesDisc(Disc d) {
		if(controlDiscRadius) d.Radius = Mathf.Min(o_Rect.rect.width, o_Rect.rect.height)/2f;
		if(controlDiscThickness) d.Thickness = discThicknessScale*Mathf.Min(o_Rect.rect.width, o_Rect.rect.height)/2f;
#if UNITY_EDITOR
		EditorUtility.SetDirty(d);
		EditorUtility.SetDirty(gameObject);
#endif
	}

	public void ConformShapesPolygon(Polygon p) {
		//o_Rect.pivot = new Vector2(0.5f, 0.5f);

		//Done to set the size of the collection
		p.SetPoints(polyPoints);

		Vector2 size = o_Rect.rect.size;
		float offset = polygonOriginIsCorner ? -0.5f : 0.0f;

		for(int i = 0; i < polyPoints.Count; i++) {
			p.SetPointPosition(i, new Vector2(size.x*(polyPoints[i].x+offset), size.y*(polyPoints[i].y+offset)));
		}

#if UNITY_EDITOR
		EditorUtility.SetDirty(p);
		EditorUtility.SetDirty(gameObject);
#endif
	}

	public void ConformShapesRectangle(Rectangle r) {
		o_Rect.pivot = new Vector2(0.5f, 0.5f);
		r.Pivot = RectPivot.Center;
		r.Height = o_Rect.rect.height;
		r.Width = o_Rect.rect.width;

		float smallerDimension = o_Rect.rect.width < o_Rect.rect.height ? o_Rect.rect.width : o_Rect.rect.height;
		float largerDimension = o_Rect.rect.width > o_Rect.rect.height ? o_Rect.rect.width : o_Rect.rect.height;


		if(affectShapesRectangleGradient) {
			switch(linearGradientDirection) {
				case LinearGradientDirection.UP:
					r.FillLinearStart = Vector3.up*(-o_Rect.rect.height/2f);
					r.FillLinearEnd = Vector3.up*(o_Rect.rect.height/2f);
					break;
				case LinearGradientDirection.DOWN:
					r.FillLinearStart = Vector3.down*(-o_Rect.rect.height/2f);
					r.FillLinearEnd = Vector3.down*(o_Rect.rect.height/2f);
					break;
				case LinearGradientDirection.LEFT:
					r.FillLinearStart = Vector3.left*(-o_Rect.rect.width/2f);
					r.FillLinearEnd = Vector3.left*(o_Rect.rect.width/2f);
					break;
				case LinearGradientDirection.RIGHT:
					r.FillLinearStart = Vector3.right*(-o_Rect.rect.width/2f);
					r.FillLinearEnd = Vector3.right*(o_Rect.rect.width/2f);
					break;
				default:
					break;
			}
			r.FillRadialRadius = radialGradientNormalizedSize*largerDimension;
		}

		if(affectShapesRectangleCorners) {
			float radiusUL = smallerDimension*cornerRadiusUL;
			float radiusUR = smallerDimension*cornerRadiusUR;
			float radiusLL = smallerDimension*cornerRadiusLL;
			float radiusLR = smallerDimension*cornerRadiusLR;
			r.CornerRadii = new Vector4(radiusLL, radiusUL, radiusUR, radiusLR);
		}

#if UNITY_EDITOR
		EditorUtility.SetDirty(r);
		EditorUtility.SetDirty(gameObject);
#endif
	}

	public void ConformShapesLine(Line l) {
		float smallerDimension = o_Rect.rect.width < o_Rect.rect.height ? o_Rect.rect.width : o_Rect.rect.height;
		if(controlLinePosition) {
			l.Start = new Vector3((lineAnchorStart.x*o_Rect.rect.width)-(o_Rect.rect.width/2f), (lineAnchorStart.y*o_Rect.rect.height)-(o_Rect.rect.height/2f));
			l.End = new Vector3((lineAnchorEnd.x*o_Rect.rect.width)-(o_Rect.rect.width/2f), (lineAnchorEnd.y*o_Rect.rect.height)-(o_Rect.rect.height/2f));
		}
		if(controlLineThickness) {
			l.Thickness = lineThickness*smallerDimension;
		}
		if(pixelOffsetX != 0 || pixelOffsetY != 0) {
			Debug.LogError("Line pixel offsets are not supported yet - disabling");
			pixelOffsetX = 0;
			pixelOffsetY = 0;
		}
#if UNITY_EDITOR
		EditorUtility.SetDirty(l);
		EditorUtility.SetDirty(gameObject);
#endif
	}


	public void ConformBoxCollider(BoxCollider2D b) {
		b.size = o_Rect.rect.size;
		b.offset = Vector2.zero;
#if UNITY_EDITOR
		EditorUtility.SetDirty(b);
		EditorUtility.SetDirty(gameObject);
#endif
	}

	public void ConformCircleCollider(CircleCollider2D c) {
		c.radius = Mathf.Min(o_Rect.rect.width, o_Rect.rect.height)/2f;
#if UNITY_EDITOR
		EditorUtility.SetDirty(c);
		EditorUtility.SetDirty(gameObject);
#endif
	}

	public void ConformSpriteRenderer(SpriteRenderer sr) {

		if(sr.sprite == null) return;
		if(sr.sprite.pixelsPerUnit == 0) return;
		float spriteWidthUnits = sr.sprite.rect.width/sr.sprite.pixelsPerUnit;
		float spriteHeightUnits = sr.sprite.rect.height/sr.sprite.pixelsPerUnit;

		if(spriteWidthUnits == 0) return;
		if(spriteHeightUnits == 0) return;

		float spriteWidthScaleM = o_Rect.rect.width/spriteWidthUnits;
		float spriteHeightScaleM = o_Rect.rect.height/spriteHeightUnits;

		if(preserveSpriteAspect) {
			spriteWidthScaleM = spriteWidthScaleM <= spriteHeightScaleM ? spriteWidthScaleM : spriteHeightScaleM;
			spriteHeightScaleM = spriteHeightScaleM <= spriteWidthScaleM ? spriteHeightScaleM : spriteWidthScaleM;
		}

		transform.localScale = new Vector3(spriteWidthScaleM, spriteHeightScaleM, transform.localScale.z);

#if UNITY_EDITOR
		EditorUtility.SetDirty(sr);
		EditorUtility.SetDirty(gameObject);
#endif
	}

	public void ConformParticleSystem(ParticleSystem ps) {
		float scale = Mathf.Max(o_Rect.rect.width, o_Rect.rect.height)*particleScale;
		transform.localScale = new Vector3(scale, scale, transform.localScale.z);
#if UNITY_EDITOR
		EditorUtility.SetDirty(ps);
		EditorUtility.SetDirty(gameObject);
#endif
	}

	public enum LinearGradientDirection : int { UP, DOWN, LEFT, RIGHT }
}

