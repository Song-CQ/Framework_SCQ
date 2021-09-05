//// QuickFill.cpp
////
//// Author : John R. Shaw (shawj2@earthlink.net)
//// Date   : Jan. 26 2004
////
//// Copyright (C) 2004 John R. Shaw
//// All rights reserved.
////
//// This code may be used in compiled form in any way you desire. This
//// file may be redistributed unmodified by any means PROVIDING it is 
//// not sold for profit without the authors written consent, and 
//// providing that this notice and the authors name is included. If 
//// the source code in this file is used in any commercial application 
//// then a simple email would be nice.
////
//// Warranties and Disclaimers:
//// THIS SOFTWARE IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND
//// INCLUDING, BUT NOT LIMITED TO, WARRANTIES OF MERCHANTABILITY,
//// FITNESS FOR A PARTICULAR PURPOSE AND NON-INFRINGEMENT.
//// IN NO EVENT WILL JOHN R. SHAW BE LIABLE FOR ANY DIRECT,
//// INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY OR CONSEQUENTIAL DAMAGES,
//// INCLUDING DAMAGES FOR LOSS OF PROFITS, LOSS OR INACCURACY OF DATA,
//// INCURRED BY ANY PERSON FROM SUCH PERSON'S USAGE OF THIS SOFTWARE
//// EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGES.
////
//// Please email bug reports, bug fixes, enhancements, requests and
//// comments to: shawj2@earthlink.net


//using System;
//using System.Drawing;

//namespace Endogine.BitmapHelpers.FloodFill
//{
//    /// <summary>
//    /// Summary description for QuickFillX.
//    /// </summary>
//    public class QuickFill
//    {
//        private static byte[] _SolidMask = new byte[8] {0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF};
//        private static byte[] _BitMask = new byte[8]{0x80,0x40,0x20,0x10,0x08,0x04,0x02,0x01};

////		struct HLINE_NODE //hlineNode_st 
////		{
////			int x1,x2,y,dy;
////			HLINE_NODE pNext, pPrev;
////		};
////		
////		struct BLOCK_NODE //blockNode_st 
////		{
////			int x1,x2,y1,y2;
////			BLOCK_NODE pNext;
////		};
////		
////		BLOCK_NODE	m_pVisitList;
////		HLINE_NODE	m_pLineList;
////		HLINE_NODE	m_pFreeList;


//        bool	m_bXSortOn;
//        bool	m_bMemError;
//        int		m_LastY;
	
//        Bitmap m_DibData;
//        Bitmap m_DibPattern;
//        //CDibData m_DibData;
//        //CDibData m_DibPattern;
//        int m_xPatMod;
//        int m_yPatMod;
//        Color m_clrPatClear;
//        byte[] m_FillMask = new byte[8];
//        Color CLR_INVALID = Color.FromArgb(0,0,0); //TODO: null...

////----------------------------------------------------------------------------
//// Public methods
////----------------------------------------------------------------------------

///* Arguments:
// *		Pointer to bitmap to fill, (x,y) coordinates of seed point,
// *		color used for solid or masked fills, border color used if area to
// *		be filled is outlined (or CLR_INVALID).
// *
// * Returns:
// *		 0 = Success.
// *		-1 = Invalid seed point.
// *		-2 = Memory allocation error.
// *		-3 = Invalid bitmap or unknown error.
// */
//        //JB: throws exceptions instead of returning error codes
//        public QuickFill(Bitmap bmp, int x, int y, Color fill_color, Color border_color) //border_color = CLR_INVALID
//        {
//            for (int i=0; i<8; i++)
//                m_FillMask[i] = 0xFF;

//            m_xPatMod = m_yPatMod = 0;
//            m_clrPatClear = CLR_INVALID;

//            Color ThisColor;
//            int MaxY,MaxX; //,dy;
//            int ChildLeft,ChildRight;
//            int ParentLeft,ParentRight;

//            //			// Create dib data object
//            //			if( !m_DibData.CreateDIB(pBitmap) )
//            //				return -3;

//            /* Initialize global variables */
//            //#ifdef QUICKFILL_TEST
//            //			SHORT nKeyState;
//            //			m_CurStackSize = m_MaxStackSize = m_VisitSize = 0U;
//            //			m_CurrentLine = 0;
//            //#endif
//            m_bXSortOn = m_bMemError = false;
//            m_LastY = -1;

//            /* Check color at x,y position */
//            ThisColor = bmp.GetPixel(x,y);

//            /* Initialize internal info based on fill type */
//            if( CLR_INVALID != border_color ) 
//            {
//                if( ThisColor == border_color )
//                    throw new Exception("Invalid seed point");

//                ThisColor = border_color;
//                m_bXSortOn = true;
//            }
//            else 
//            {
//                if (ThisColor == fill_color) // && !m_DibPattern.GetDibPtr()
//                    throw new Exception("Invalid seed point");

//                if (CompareArray(m_FillMask,_SolidMask,8)) // || m_DibPattern.GetDibPtr()
//                    m_bXSortOn = true;
//            }

//            /* Initialize Line list */
//            MakeList();

//            /* Initialize maximum coords */
//            MaxX = m_DibData.Width-1;
//            MaxY = m_DibData.Height-1;

//            //	/* Push starting point on stack */
//            //	PushLine(x,x,y,+1);		/* Needed in one special case */
//            //	PushLine(x,x,y+1,-1);

//        }

////
////	/* Now start flooding */
////	while( m_pLineList ) {
////		PopLine(&ParentLeft,&ParentRight,&y,&dy);
////		y += dy;
////		if( y < 0 || MaxY < y )
////			continue;
////		if( m_bMemError )
////			continue;
////		if( m_bXSortOn && IsRevisit(ParentLeft,ParentRight,y) )
////			continue;
////
////
////		/* Find ChildLeft end ( ChildLeft>ParentLeft on failure ) */
////		ChildLeft = FindLeft(ParentLeft,y,0,ThisColor)+1;
////		if( ChildLeft<=ParentLeft ) {
////			/* Find ChildRight end ( this should not fail here ) */
////			ChildRight = FindRight(ParentLeft,y,MaxX,ThisColor)-1;
////			/* Fill line */
////			if( ChildLeft == ChildRight )
////				SetPixel(ChildRight,y,fill_color);
////			else
////				DrawHorizontalLine(ChildLeft,ChildRight,y,fill_color);
////
////
////			/* Push unvisited lines */
////			if( ParentLeft-1<=ChildLeft && ChildRight<=ParentRight+1 ) {
////				PushLine(ChildLeft,ChildRight,y,dy);
////			}
////			else {
////				if( m_bXSortOn )
////					PushOpposite(ParentLeft,ParentRight,ChildLeft,ChildRight,y,dy);
////				else
////					PushLine(ChildLeft,ChildRight,y,-dy);
////				PushLine(ChildLeft,ChildRight,y,dy);
////			}
////			/* Addvance ChildRight end on to border */
////			++ChildRight;
////		}
////		else ChildRight = ParentLeft;
////
////		/* Fill betweens */
////		while( ChildRight < ParentRight ) {
////			/* Skip to new ChildLeft end ( ChildRight>ParentRight on failure ) */
////			ChildRight = SkipRight(ChildRight,y,ParentRight,ThisColor);
////			/* If new ChildLeft end found */
////			if( ChildRight<=ParentRight ) {
////				ChildLeft = ChildRight;
////				/* Find ChildRight end ( this should not fail here ) */
////				ChildRight = FindRight(ChildLeft,y,MaxX,ThisColor)-1;
////				/* Fill line */
////				if( ChildLeft == ChildRight )
////					SetPixel(ChildRight,y,fill_color);
////				else
////					DrawHorizontalLine(ChildLeft,ChildRight,y,fill_color);
////
////
////				/* Push unvisited lines */
////				if( ChildRight <= ParentRight+1 )
////					PushLine(ChildLeft,ChildRight,y,dy);
////				else {
////					if( m_bXSortOn )
////						PushOpposite(ParentLeft,ParentRight,ChildLeft,ChildRight,y,dy);
////					else
////						PushLine(ChildLeft,ChildRight,y,-dy);
////					PushLine(ChildLeft,ChildRight,y,dy);
////				}
////				/* Addvance ChildRight end onto border */
////				++ChildRight;
////			}
////		}
////
////		/* Push visited line onto visited stack */
////		if( m_bXSortOn )
////			PushVisitedLine(ParentLeft,ParentRight,y);
////	}
////	FreeList();
////	m_DibData.SetDIBits(pBitmap);
////	m_DibData.DeleteObject();
////	return( m_bMemError?-2:0 );
////}
////
////// Argument: FillMask[8] or NULL
////void CQuickFill::SetFillMask(BYTE* pMask/*=NULL*/)
////{
////	if( !pMask )
////		memcpy(m_FillMask,_SolidMask,8);
////	else
////		memcpy(m_FillMask,pMask,8);
////}
////
////// Argument: Pointer to bitmap or NULL.
////// Returns: Nonzero on success.
////BOOL CQuickFill::SetPatternBitmap(CBitmap* pBitmap/*=NULL*/,COLORREF clrClear/*=CLR_INVALID*/)
////{
////	m_DibPattern.DeleteObject();
////	m_xPatMod = m_yPatMod = 0;
////	m_clrPatClear = CLR_INVALID;
////	if( pBitmap ) {
////		if( !m_DibPattern.CreateDIB(pBitmap) )
////			return FALSE;
////		m_xPatMod = m_DibPattern.GetWidth()-1;
////		m_yPatMod = m_DibPattern.GetHeight()-1;
////		m_clrPatClear = clrClear;
////	}
////	return TRUE;
////}
////
////// Argument: Handle to bitmap or NULL.
////// Returns: Nonzero on success.
////BOOL CQuickFill::SetPatternBitmap(HBITMAP hBitmap,COLORREF clrClear/*=CLR_INVALID*/)
////{
////	m_DibPattern.DeleteObject();
////	m_xPatMod = m_yPatMod = 0;
////	m_clrPatClear = CLR_INVALID;
////	if( hBitmap ) {
////		if( !m_DibPattern.CreateDIB(hBitmap) )
////			return FALSE;
////		m_xPatMod = m_DibPattern.GetWidth()-1;
////		m_yPatMod = m_DibPattern.GetHeight()-1;
////		m_clrPatClear = clrClear;
////	}
////	return TRUE;
////}
////
////// Argument: Path/Name of bitmap file.
////// Returns: Nonzero on success.
////BOOL CQuickFill::SetPatternBitmap(LPCTSTR lpszPathName,COLORREF clrClear/*=CLR_INVALID*/)
////{
////	m_DibPattern.DeleteObject();
////	m_xPatMod = m_yPatMod = 0;
////	m_clrPatClear = CLR_INVALID;
////	if( lpszPathName ) {
////		if( !m_DibPattern.LoadDIB(lpszPathName) )
////			return FALSE;
////		m_xPatMod = m_DibPattern.GetWidth()-1;
////		m_yPatMod = m_DibPattern.GetHeight()-1;
////		m_clrPatClear = clrClear;
////	}
////	return TRUE;
////}
////
////// Argument: Transparent color used with pattern bitmaps.
////// Returns: Previous color.
////COLORREF CQuickFill::SetPatternClearColor(COLORREF clrClear/*=CLR_INVALID*/)
////{
////	COLORREF color = m_clrPatClear;
////	m_clrPatClear = clrClear;
////	return color;
////}
////
////// Returns: Nonzero if pattern bitmap is being used.
////BOOL CQuickFill::HasPattern()
////{
////	return( m_xPatMod && m_yPatMod );
////}
////
////// Returns: Nonzero if fill masks are being used.
////BOOL CQuickFill::HasMask()
////{
////	return( !memcmp(m_FillMask,_SolidMask,8) );
////}
////
//////----------------------------------------------------------------------------
////// Public Constructor/Destructor methods
//////----------------------------------------------------------------------------
////
////CQuickFill::CQuickFill()
////{
////	m_pVisitList = NULL;
////	m_pLineList = NULL;		// List of lines to be visited.
////	m_pFreeList = NULL;		// List of free nodes.
////	m_bXSortOn = FALSE;		// X-Sorting flag (single visit indicator).
////	m_bMemError = FALSE;	// Memory error flag.
////#ifdef QUICKFILL_SLOW
////	m_bSlowMode = FALSE;
////#endif
////#ifdef QUICKFILL_TEST
////	m_CurStackSize = 0U;	// Current number of nodes in line list.
////	m_MaxStackSize = 0U;	// Greatest number of nodes added to line list.
////	m_VisitSize = 0U;
////	m_CurrentLine  = 0;		// Current line being visited
////#endif
////	memset(m_FillMask,0xFF,8);
////	m_xPatMod = m_yPatMod = 0;
////	m_clrPatClear = CLR_INVALID;
////}
////
////CQuickFill::~CQuickFill()
////{
////	// Safety only
////	FreeList();
////	m_DibData.DeleteObject();
////}
////
//////----------------------------------------------------------------------------
////// Protected scanning and drawing methods
//////----------------------------------------------------------------------------
////
////// Arguments: x, y coordinates of pixel to examine.
////// Returns: Color of pixel or CLR_INVALID on failure.
////COLORREF CQuickFill::GetPixel(int x, int y)
////{
////	return m_DibData.GetPixel(x,y);
////}
////
////// Arguments: x, y coordinates and new color of pixel.
////BOOL CQuickFill::SetPixel(int x, int y, COLORREF color)
////{
////	if( _BitMask[x&7] & m_FillMask[y&7] ) {
////		if( m_xPatMod && m_yPatMod ) {
////			color = m_DibPattern.GetPixel(x%m_xPatMod,y%m_yPatMod);
////			if( m_clrPatClear != CLR_INVALID && color == m_clrPatClear ) {
////				return FALSE;
////			}
////			return m_DibData.SetPixel(x,y,color);
////		}
////		else
////			return m_DibData.SetPixel(x,y,color);
////	}
////	return FALSE;
////}
////
////// Arguments: Coordinates of horizontal line and new color of line.
////void CQuickFill::DrawHorizontalLine(int x1, int x2, int y, COLORREF color)
////{
////	for( ; x1 <= x2; ++x1 )
////		SetPixel(x1,y,color);
////}
////

///*	ScanLeft()
// *	xmin > result -> failure
// */
//        private int ScanLeft(int x, int y, int xmin, Color color)
//        {
//            for( ; x >= xmin; --x )
//            {
//                if( m_DibData.GetPixel(x,y) != color )
//                    break;
//            }
//            return x;
//        }

///*	SearchLeft()
// *	xmin > result -> failure
// */
//        private int SearchLeft(int x, int y, int xmin, Color color)
//        {
//            for( ; x >= xmin; --x )
//            {
//                if( m_DibData.GetPixel(x,y) == color )
//                    break;
//            }
//            return x;
//        }

///*	ScanRight()
// *	xmax < result -> failure
// */
//        private int ScanRight(int x, int y, int xmax, Color color)
//        {
//            for( ; x <= xmax; ++x )
//            {
//                if( m_DibData.GetPixel(x,y) != color )
//                    break;
//            }
//            return x;
//        }
		
///*	SearchRight()
// *	xmax < result -> failure
// */
//        private int SearchRight(int x,int y,int xmax,Color color)
//        {
//            for( ; x <= xmax; ++x )
//            {
//                if( m_DibData.GetPixel(x,y) == color )
//                    break;
//            }
//            return x;
//        }

//////----------------------------------------------------------------------------
////// Private methods
//////----------------------------------------------------------------------------

///* Initialize Line list */
//        private void MakeList()
//        {
////			m_pFreeList = (HLINE_NODE*)calloc(1,sizeof(HLINE_NODE));
////			return m_pFreeList?false:true;
//            throw new Exception("Out of memory for makelist");
//        }

///* Frees the list of free nodes */
//        private void FreeList()
//        {
////			HLINE_NODE *pNext;
////			while( m_pFreeList ) 
////			{
////				pNext = m_pFreeList->pNext;
////				free(m_pFreeList);
////				m_pFreeList = pNext;
////			}
////
////			while( m_pLineList ) 
////			{
////				pNext = m_pLineList->pNext;
////				free(m_pLineList);
////				m_pLineList = pNext;
////			}
////	
////			BLOCK_NODE *pNextBlock;
////			while( m_pVisitList ) 
////			{
////				pNextBlock = m_pVisitList->pNext;
////				free(m_pVisitList);
////				m_pVisitList = pNextBlock;
////			}
//        }

/////* Push a node onto the line list */
////void CQuickFill::PushLine(int x1,int x2,int y,int dy)
////{
////	HLINE_NODE *pNew = m_pFreeList;
////	if( pNew )
////		m_pFreeList = m_pFreeList->pNext;
////	else
////		pNew = (HLINE_NODE*)calloc(1,sizeof(HLINE_NODE));
////	/* Add to start of list */
////	if( pNew ) {
////		pNew->x1 = x1;
////		pNew->x2 = x2;
////		pNew->y  = y;
////		pNew->dy = dy;
////		
////		if( m_bXSortOn ) {
////			/* This is for the single line visiting code.
////			 * The code runs about 18% slower but you can
////			 * use fill patterns with it.
////			 * Since the basic algorithym scans lines from
////			 * left to right it is required that the list
////			 * be sorted from left to right.
////			 */
////			HLINE_NODE *pThis,*pPrev=(HLINE_NODE*)0;
////			for( pThis=m_pLineList;pThis;pThis=pThis->pNext ) {
////				if( x1 <= pThis->x1 )
////					break;
////				pPrev = pThis;
////			}
////			if( pPrev ) {
////				pNew->pNext = pPrev->pNext;
////				pNew->pPrev = pPrev;
////				pPrev->pNext = pNew;
////				if( pNew->pNext )
////					pNew->pNext->pPrev = pNew;
////			}
////			else {
////				pNew->pNext = m_pLineList;
////				pNew->pPrev = (HLINE_NODE*)0;
////				if( pNew->pNext )
////					pNew->pNext->pPrev = pNew;
////				m_pLineList = pNew;
////			}
////		}
////		else {
////			pNew->pNext = m_pLineList;
////			m_pLineList = pNew;
////		}
////#ifdef QUICKFILL_TEST
////		++m_CurStackSize;
////		if( m_CurStackSize > m_MaxStackSize )
////			m_MaxStackSize = m_CurStackSize;
////#endif
////	}
////	else m_bMemError = 1;
////}
////
/////* Pop a node off the line list */
////void CQuickFill::PopLine(int *x1,int *x2,int *y,int *dy)
////{
////	if( m_pLineList ) {
////		HLINE_NODE *pThis,*pPrev;
////		/* Search lines on stack for same line as last line.
////		 * This smooths out the flooding of the graphics object
////		 * and reduces the size of the stack.
////		 */
////		pPrev = m_pLineList;
////		for( pThis=m_pLineList->pNext;pThis;pThis=pThis->pNext ) {
////			if( pThis->y == m_LastY )
////				break;
////			pPrev = pThis;
////		}
////		/* If pThis found - remove it from list */
////		if( pThis ) {
////			pPrev->pNext = pThis->pNext;
////			if( pPrev->pNext )
////				pPrev->pNext->pPrev = pPrev;
////			*x1 = pThis->x1;
////			*x2 = pThis->x2;
////			*y  = pThis->y;
////			*dy = pThis->dy;
////		}
////		/* Remove from start of list */
////		else {
////			*x1 = m_pLineList->x1;
////			*x2 = m_pLineList->x2;
////			*y  = m_pLineList->y;
////			*dy = m_pLineList->dy;
////			pThis = m_pLineList;
////			m_pLineList = m_pLineList->pNext;
////			if( m_pLineList )
////				m_pLineList->pPrev = (HLINE_NODE*)0;
////		}
////		pThis->pNext = m_pFreeList;
////		m_pFreeList = pThis;
////#ifdef QUICKFILL_TEST
////		--m_CurStackSize;
////#endif
////		m_LastY = *y;
////	}
////}
////
/////***** Single Visit Code *******************************************/
////
/////* Jan 24, 2004
//// * Testing showed a gapping hole in the push opposite code, that
//// * would cause QuickFill to get stuck. The cause of this was the
//// * fact that push opposite just reduced the number of revisits but
//// * did not stop them.
//// */
////
/////* Adds line to visited block list */
////void CQuickFill::PushVisitedLine(int x1,int x2,int y)
////{
////	if( !ExpandVisitedBlock(x1,x2,y) ) {
////		BLOCK_NODE *pNew = (BLOCK_NODE*)calloc(1,sizeof(BLOCK_NODE));
////		/* Add to start of list */
////		if( pNew ) {
////			pNew->x1 = x1;
////			pNew->x2 = x2;
////			pNew->y1 = y;
////			pNew->y2 = y;
////			pNew->pNext = m_pVisitList;
////			m_pVisitList = pNew;
////#ifdef QUICKFILL_TEST
////			++m_VisitSize;
////#endif
////		}
////		else m_bMemError = 1;
////	}
////}
////
/////* Expands block if need to reduce number
//// * of block allocations (by abount 1/2).
//// */
////BOOL CQuickFill::ExpandVisitedBlock(int x1,int x2,int y)
////{
////	BLOCK_NODE* pNext = m_pVisitList;
////	while( pNext ) {
////		/* If line is in blocks x range */
////		if( pNext->x1 <= x1 && x2 <= pNext->x2 ) {
////			/* Try to expand verticaly */
////			if( pNext->x1 == x1 && x2 == pNext->x2 ) {
////				if( pNext->y1-1 == y )
////					pNext->y1 = y;
////				if( pNext->y2+1 == y )
////					pNext->y2 = y;
////			}
////			/* Test if in block */
////			if( pNext->y1 <= y && y <= pNext->y2 )
////				break;
////		}
////		/* If line is in blocks y range */
////		else if( pNext->y1 <= y && y <= pNext->y2 ) {
////			/* Try to expand horizontaly */
////			if( pNext->x1-1 == x1 ) {
////				pNext->x1 = x1;
////			}
////			if( pNext->x2+1 == x2 ) {
////				pNext->x2 = x2;
////			}
////			/* Test if in block */
////			if( pNext->x1 <= x1 && x2 <= pNext->x2 )
////				break;
////		}
////		pNext = pNext->pNext;
////	}
////	return( pNext != NULL );
////}
////
/////* Checks if line has already been visited */
////BOOL CQuickFill::IsRevisit(int x1,int x2,int y)
////{
////	BLOCK_NODE* pNext = m_pVisitList;
////	while( pNext ) {
////		if( pNext->y1 <= y && y <= pNext->y2 ) {
////			if( pNext->x1 <= x1 && x2 <= pNext->x2 )
////				break;
////		}
////		pNext = pNext->pNext;
////	}
////	return( pNext != NULL );
////}
////
/////* Find next line segment on parent line.
//// * Note: This function is designed to be
//// *		called until NULL is returned.
//// */
////CQuickFill::HLINE_NODE* CQuickFill::FindNextLine(int x1,int x2,int y)
////{
////	static HLINE_NODE *pFindNext=(HLINE_NODE*)0;
////	HLINE_NODE *pThis;
////	if( !pFindNext )
////		pFindNext = m_pLineList;
////	for( pThis=pFindNext;pThis;pThis=pThis->pNext ) {
////		if( (pThis->y+pThis->dy) == y ) {
////			if( x1 < pThis->x1 && pThis->x1 <= x2 ) {
////				pFindNext = pThis->pNext;
////				return pThis;
////			}
////		}
////	}
////	return( pFindNext=(HLINE_NODE*)0 );
////}
////
/////* Removes pThis from line list */
////void CQuickFill::PopThis(HLINE_NODE *pThis)
////{
////	if( m_pLineList ) {
////		/* If pThis is not start of list */
////		if( pThis->pPrev ) {
////			HLINE_NODE *pPrev = pThis->pPrev;
////			pPrev->pNext = pThis->pNext;
////			if( pPrev->pNext )
////				pPrev->pNext->pPrev = pPrev;
////		}
////		/* Remove pThis from start of list */
////		else {
////			m_pLineList = m_pLineList->pNext;
////			if( m_pLineList )
////				m_pLineList->pPrev = (HLINE_NODE*)0;
////		}
////		pThis->pNext = m_pFreeList;
////		m_pFreeList = pThis;
////#ifdef QUICKFILL_TEST
////		--m_CurStackSize;
////#endif
////	}
////}
////
/////* Push unvisited lines onto the stack */
////void CQuickFill::PushOpposite(int OldX1,int OldX2,int x1,int x2,int y,int dy)
////{
////	/* Find next line on parent line */
////	HLINE_NODE *pFind = FindNextLine(x1,x2,y);
////	if( !pFind ) {
////		/* push cliped left ends */
////		if( x1 < --OldX1 )
////			PushLine(x1,--OldX1,y,-dy);
////		if( x2 > ++OldX2 )
////			PushLine(++OldX2,x2,y,-dy);
////	}
////	else {
////		/* push cliped left */
////		if( x1 < --OldX1 )
////			PushLine(x1,--OldX1,y,-dy);
////		/* set test value for right cliping */
////		OldX1 = x2+1;
////		do {
////			/* push valid line only */
////			if( ++OldX2 < pFind->x1-2 )
////				PushLine(++OldX2,pFind->x1-2,y,-dy);
////			OldX2 = pFind->x2;
////			/* clip right end if needed */
////			if( OldX2 > OldX1 ) {
////				pFind->x1 = ++OldX1;
////			}
////			else /* pop previously visited line */
////				PopThis(pFind);
////			pFind = FindNextLine(x1,x2,y);
////		} while( pFind );
////	}
////}


//        private int FindLeft(int x, int y, int xmin, Color color, Color border_color)
//        {
//            if (border_color != CLR_INVALID)
//                return SearchLeft(x,y,xmin,color);
//            return ScanLeft(x,y,xmin,color);
//        }
//        private int FindRight(int x, int y, int xmax, Color color, Color border_color)
//        {
//            if (border_color != CLR_INVALID)
//                return SearchRight(x,y,xmax,color);
//            return ScanRight(x,y,xmax,color);
//        }
//        private int SkipRight(int x, int y, int xmax, Color color, Color border_color)
//        {
//            if (border_color != CLR_INVALID)
//                return ScanRight(x,y,xmax,color);
//            return SearchRight(x,y,xmax,color);
//        }

//        private bool CompareArray(byte[] a, byte[] b, int numbytes)
//        {
//            for (int i = 0; i < numbytes; i++)
//                if (a[i] != b[i])
//                    return false;
//            return true;
//        }

//    }
//}
